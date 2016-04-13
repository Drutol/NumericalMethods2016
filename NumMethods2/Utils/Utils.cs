using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumMethods2.MatrixMath;

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

        public static T[,] ResizeArray<T>(T[,] original, int rows, int cols)
        {
            var newArray = new T[rows, cols];
            int minRows = Math.Min(rows, original.GetLength(0));
            int minCols = Math.Min(cols, original.GetLength(1));
            for (int i = 0; i < minRows; i++)
                for (int j = 0; j < minCols; j++)
                    newArray[i, j] = original[i, j];
            return newArray;
        }

        public static double[] SwapResultsRows(double[] source, int rowFrom, int rowTo)
        {
            double elem = source[rowFrom];
            source[rowFrom] = source[rowTo];
            source[rowTo] = elem;
            return source;
        }

        public static double[,] SwapMatrixRows(double[,] source,int row,int size)
        {
            List<List<double>> list = new List<List<double>>();
            var flatMatrix = source.Cast<double>();
            for (int i = 0; i < size; i++)
            {
                var matrixRow = flatMatrix.Skip(i*size).Take(size).ToList();
                matrixRow.Add(0);
                list.Add(matrixRow);
            }

            for(int i = 1; i < size - row;i++)
                if (list[row + i][row + i] != 0)
                {
                    NumCore.RowNum = row + i;
                    var listRow = list[row];
                    list.RemoveAt(row);
                    list.Insert(row + i, listRow);
                    return Utils.To2DArray<double>(list);
                }


            throw new Exception("Nieoznaczon");

        }
    }
}
