﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using System.Threading;

namespace Rezgar.Utils.Data
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TDataType">Must be decorated with [Serializable] attribute</typeparam>
    public sealed class DataCollector<TDataType> : IDisposable
        where TDataType: class
    {
        #region Static initialization

        // NOTE: Not using a static DirectoryInfo because of the caching. It thinks the directory still exists, when it does not and that there are files, when they are deleted
        private static readonly string _tempFilesDirectoryPath;

        static DataCollector()
        {
            _tempFilesDirectoryPath = System.IO.Path.Combine("../temp/data_collector/", typeof(TDataType).Name);
            
            if (!Directory.Exists(_tempFilesDirectoryPath))
                Directory.CreateDirectory(_tempFilesDirectoryPath);
        }

        #endregion

        #region Fields/Properties

        public DateTime LastCollectedOn {get; private set;}
        public event Action<DateTime, ICollection<TDataType>> Collected;

        // Data storage
        private readonly ConcurrentBag<TDataType> _data;

        // Timer for dumping collected data
        private readonly System.Timers.Timer _collectionTimer = new System.Timers.Timer();

        #endregion

        /// <summary>
        /// TODO: Consider using singleton to avoid muliple instances, picking up temp files from one another
        /// </summary>
        public DataCollector()
        {
            // Restore data from temp files. If they are present on disk, means they were not processed in previous run (processing hardly cancelled by user or due to hard pc reset)
            var restoredData = RestoreFromTempFiles();
            _data = new ConcurrentBag<TDataType>(restoredData);
        }
        
        /// <summary>
        /// We don't necessarily need to "wait" for the task to complete in most of the cases. Just Fire-and-forget.
        /// </summary>
        /// <returns>Temp file name</returns>
        public async Task<string> RegisterAsync(TDataType data)
        {
            _data.Add(data);
            return await RegisterInTempFileAsync(data)
                .ContinueWith(tempFileRegistrationTask =>
                {
                    if (tempFileRegistrationTask.Status != TaskStatus.RanToCompletion)
                    {
                        Trace.TraceWarning($"DataCollector<{typeof(TDataType).Name}>.RegisterAsync: Temp file creation failed with exception [{tempFileRegistrationTask.Exception}]. Not critical.");
                        return null;
                    }

                    return tempFileRegistrationTask.Result;
                });
        }

        #region Collection

        public void ScheduleCollection(TimeSpan periodTimeSpan, Func<IList<TDataType>, Task<bool>> onDataCollectedAsync)
        {
            _collectionTimer.Enabled = false;
            _collectionTimer.Interval = periodTimeSpan.TotalMilliseconds;

            _collectionTimer.Elapsed += (sender, eventArgs) =>
            {
                var data = Collect();

                var success = false;
                try
                {
                    // TODO: Find a way for true async, using timer or something else
                    success = onDataCollectedAsync(data).Result;
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"DataCollector<{typeof(TDataType).Name}>.ScheduleCollection.Collect: Data portion of {data?.Count} records failed with exception {ex}. Re-adding failed records to collection.");
                }

                if (!success && data != null)
                {
                    foreach (var record in data)
                    {
                        _data.Add(record);
                    }
                }
            };
            _collectionTimer.Enabled = true;
        }

        // Dump collected portion method
        public IList<TDataType> Collect()
        {
            var dataForCollection = new List<TDataType>(_data.Count);
            while (_data.TryTake(out var dataRecord))
            {
                dataForCollection.Add(dataRecord);
            }

            try
            {
                CleanupTempFiles();
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}: Cleanup temp files failed with exception [{1}]", GetType().Name, ex);
            }

            LastCollectedOn = DateTime.UtcNow;
            Collected?.Invoke(DateTime.UtcNow, dataForCollection);

            return dataForCollection;
        }

        #endregion

        #region File-based layer to ensure queue items are never lost

        private async Task<string> RegisterInTempFileAsync(TDataType data)
        {
            var fileName = $"{DateTime.UtcNow.Ticks}_{typeof(TDataType).Name}.dmp.temp";
            var filePath = Path.Combine(_tempFilesDirectoryPath, fileName);

            var serializer = new JsonSerializer();
            using (var fileStream = File.Create(filePath))
            {
                using (var sr = new StreamWriter(fileStream))
                using (var writer = new JsonTextWriter(sr))
                {
                    serializer.Serialize(writer, data);
                    await fileStream.FlushAsync();
                }
            }

            return fileName;
        }

        private void CleanupTempFiles()
        {
            if (Directory.Exists(_tempFilesDirectoryPath))
            {
                //Delete all files from the Directory
                foreach (string file in Directory.GetFiles(_tempFilesDirectoryPath))
                {
                    File.Delete(file);
                }
            }
        }

        private IList<TDataType> RestoreFromTempFiles()
        {
            var result = new List<TDataType>();

            var serializer = new JsonSerializer();
            if (Directory.Exists(_tempFilesDirectoryPath))
                foreach (var filePath in Directory.GetFiles(_tempFilesDirectoryPath))
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        using (var sr = new StreamReader(stream))
                        using (var jsonTextReader = new JsonTextReader(sr))
                            result.Add(serializer.Deserialize<TDataType>(jsonTextReader));
                    }
                }

            return result;
        }
        
        #endregion

        #region IDisposable

        public void Dispose()
        {
            // collect one last time, in order not to lose data
            if (_collectionTimer.Enabled)
            {
                var semaphore = new SemaphoreSlim(0, 1);

                _collectionTimer.Elapsed += (sender, e) =>
                {
                    _collectionTimer.Enabled = false;
                    semaphore.Release();
                };

                semaphore.Wait();
            }

            _collectionTimer.Dispose();

            CleanupTempFiles();
        }

        #endregion
    }
}
