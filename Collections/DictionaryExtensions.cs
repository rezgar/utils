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
    }
}
