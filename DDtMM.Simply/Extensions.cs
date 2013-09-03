using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY
{
    static class Extensions
    {
        public static T Next<T>(this IList<T> list, T obj) 
        {
            int index = list.IndexOf(obj);
            return (index > -1 && index < list.Count - 1) ? list[index++] : default(T);
        }

        public static T Prev<T>(this IList<T> list, T obj)
        {
            int index = list.IndexOf(obj);
            return (index > 0) ? list[index--] : default(T);
        }
    }
}
