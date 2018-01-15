using System;
using System.Collections.Generic;
using System.Text;

namespace Rezgar.Utils.Collections
{
    public static class IListExtensions
    {
        public static TElement GetRandomElement<TElement>(this IList<TElement> list)
        {
            var random = new Random();
            return list[random.Next(0, list.Count - 1)];
        }
    }
}
