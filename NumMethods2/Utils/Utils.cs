using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods2
{
    public static class Utils
    {
        public static T[,] To2DArray<T>(this List<List<T>> source)
        { 
            int max = source.Select(l => l).Max(l => l.Count());

            var result = new T[source.Count, max];

            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < source[i].Count(); j++)
                {
                    result[i, j] = source[i][j];
                }
            }

            return result;
        }
    }
}
