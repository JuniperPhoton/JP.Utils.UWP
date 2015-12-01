using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.Utils.Data
{
    public static class ICollectionExtension
    {
        public static ICollection<T> Adding<T>(this ICollection<T> list,T item)
        {
            list.Add(item);
            return list;
        }
    }
}
