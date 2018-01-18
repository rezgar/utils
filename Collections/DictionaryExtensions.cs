using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq;

namespace Rezgar.Utils.Collections
{
    public static class DictionaryExtensions
    {
        public static NameValueCollection ToNameValueCollection<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return dictionary
            .Aggregate(new NameValueCollection(),
                (seed, current) =>
                {
                    seed.Add(current.Key.ToString(), current.Value.ToString());
                    return seed;
                });
        }

        public static IDictionary<TKey, TValue> MergeWith<TKey, TValue>(this IDictionary<TKey, TValue> dictionary1, params IDictionary<TKey, TValue>[] dictionaries)
        {
            return Merge(Enumerable.Repeat(dictionary1, 1).Concat(dictionaries).ToArray());
        }
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(params IDictionary<TKey, TValue>[] dictionaries)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var dict in dictionaries)
                foreach (var x in dict)
                    result[x.Key] = x.Value;

            return result;
        }
    }
}
