using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI.Data
{
    static class EnumHelper
    {
        public static (T min, T max) MinMaxBy<T, U>(this IEnumerable<T> ts, Func<T, U> mapper) where U : IComparable<U>
        {
            T min, max;

            min = max = ts.First();

            foreach (T item in ts)
            {
                min = mapper(min).CompareTo(mapper(item)) < 0 ? min : item;
                max = mapper(max).CompareTo(mapper(item)) > 0 ? max : item;
            }

            return (min, max);
        }
    }
}
