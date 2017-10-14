using System.Collections.Generic;

namespace JP.Utils.Data
{
    public static class ICollectionExtension
    {
        public static ICollection<T> Adding<T>(this ICollection<T> list, T item)
        {
            list.Add(item);
            return list;
        }
    }
}