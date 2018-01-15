using System;
using System.Collections.Generic;
using System.Linq;

namespace Rezgar.Utils.Collections
{
    public class CollectionDictionary<TValue> : CollectionDictionary<string, TValue> where TValue : class
    {
        public CollectionDictionary() { }

        public CollectionDictionary(IEqualityComparer<string> comparer) : base(comparer) { }
    }

    public class CollectionDictionary<TKey, TValue> : Dictionary<TKey, IList<TValue>>
    {
        public CollectionDictionary() { }

        public CollectionDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }

        /// <summary>
        /// Adds value to the list by key or creates a new list if key not found
        /// </summary>
        public void AddValue(TKey key, TValue value, bool includeNullValues = false)
        {
            if (!includeNullValues && value == null)
                return;

            IList<TValue> dicValues;
            if (TryGetValue(key, out dicValues))
                dicValues.Add(value);
            else
                Add(key, new List<TValue> { value });
        }

        /// <summary>
        /// Add list to the dictionary or appends if key already exists
        /// </summary>
        public int AddValues(TKey key, IList<TValue> values)
        {
            if (values == null || values.Count == 0)
                return 0;

            IList<TValue> dicValues;
            if (TryGetValue(key, out dicValues))
            {
                foreach(var value in values)
                    dicValues.Add(value);
            }
            else
            {
                dicValues = values;
                this[key] = dicValues.ToList();
            }

            return values.Count;
        }

        /// <summary>
        /// Add enumerable to the dictionary or appends if key already exists
        /// </summary>
        /// <returns>Total list count with the new values added</returns>
        public int AddValues(TKey key, IEnumerable<TValue> values)
        {
            if (values == null)
                return 0;

            int count = 0;
            IList<TValue> dicValues;

            if (TryGetValue(key, out dicValues))
            {
                count = dicValues.Count;
                foreach(var value in values)
                    dicValues.Add(value);
            }
            else
            {
                dicValues = new List<TValue>(values);

                if (dicValues.Count > 0)
                    this[key] = dicValues.ToList();
            }

            return dicValues.Count - count;
        }

        /// <summary>
        /// Puts value in dictionary overwriting existing
        /// </summary>
        public void SetValue(TKey key, TValue value)
        {
            //NOTE: Make sure you don't encapsulate lists
            this[key] = new List<TValue> { value };
        }

        /// <summary>
        /// Puts list in dictionary overwriting existing
        /// </summary>
        public void SetValues(TKey key, IList<TValue> values)
        {
            this[key] = values.ToList();
        }

        public void SetValues(TKey key, IEnumerable<TValue> values)
        {
            this[key] = new List<TValue>(values);
        }

        /// <summary>
        /// Returns first value from the list by key
        /// </summary>
        public TValue GetValue(TKey key)
        {
            IList<TValue> dicValues;

            if (TryGetValue(key, out dicValues))
                if (dicValues.Count > 0)
                    return dicValues[0];

            return default(TValue);
        }

        /// <summary>
        /// Returns first non-empty string from the list by key
        /// </summary>
        public string GetNonEmptyValue(TKey key)
        {
            var dicValues = GetValues(key);

            if (dicValues != null)
            {
                return dicValues.Select(value => value as string)
                    .FirstOrDefault(stringValue => !string.IsNullOrEmpty(stringValue));
            }

            return null;
        }

        /// <summary>
        /// Returns list by key
        /// </summary>
        public IList<TValue> GetValues(TKey key)
        {
            IList<TValue> dicValues;

            if (TryGetValue(key, out dicValues))
                return dicValues;

            return null;
        }

        /// <summary>
        /// Returns first list by key value and removes list from dictionary
        /// </summary>
        public TValue PopValue(TKey key)
        {
            IList<TValue> dicValues;

            if (TryGetValue(key, out dicValues))
            {
                Remove(key);

                if (dicValues.Count > 0)
                    return dicValues[0];
            }

            return default(TValue);
        }

        /// <summary>
        /// Returns list by key and removes it from dictionary
        /// </summary>
        public IList<TValue> PopValues(TKey key)
        {
            IList<TValue> dicValues;

            if (TryGetValue(key, out dicValues))
            {
                Remove(key);
                return dicValues;
            }

            return null;
        }
    }

    public static class CollectionDictionaryExtensions
    {
        public static CollectionDictionary<TKey, TElement> ToCollectionDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, Func<TSource, IEnumerable<TElement>> elementSelector) where TElement : class
        {
            var result = new CollectionDictionary<TKey, TElement>();

            foreach (var element in source)
                result.AddValues(keySelector(element), elementSelector(element));

            return result;
        }

        public static CollectionDictionary<TKey, TElement> ToCollectionDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TElement : class
        {
            var result = new CollectionDictionary<TKey, TElement>();
            foreach (var element in source)
                result.AddValue(keySelector(element), elementSelector(element), true);

            return result;
        }
    }
}
