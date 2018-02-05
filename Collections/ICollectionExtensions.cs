using System;
using System.Collections.Generic;
using System.Text;

namespace Rezgar.Utils.Collections
{
    public static class ICollectionExtensions
    {
        public static HashSet<TElement> ToHashSet<TElement>(this ICollection<TElement> list)
        {
            return new HashSet<TElement>(list);
        }
    }
}
