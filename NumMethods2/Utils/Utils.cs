using System;
using System.Collections.Generic;
using System.Linq;

namespace NumMethods2
{
    public static class Utils
    {
        public static T[,] To2DArray<T>(this List<List<T>> source)
        {
            var max = source.Select(l => l).Max(l => l.Count());

            var result = new T[source.Count, max];

            for (var i = 0; i < source.Count; i++)
            {
                for (var j = 0; j < source[i].Count(); j++)
                {
                    result[i, j] = source[i][j];
                }
            }

            return result;
        }

        public static T[,] ResizeArray<T>(T[,] original, int rows, int cols)
        {
            var newArray = new T[rows, cols];
            var minRows = Math.Min(rows, original.GetLength(0));
            var minCols = Math.Min(cols, original.GetLength(1));
            for (var i = 0; i < minRows; i++)
                for (var j = 0; j < minCols; j++)
                    newArray[i, j] = original[i, j];
            return newArray;
        }
    }
}