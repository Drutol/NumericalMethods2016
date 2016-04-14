using System;
using System.Collections.Generic;
using System.Linq;
using NumMethods2.Exceptions;

namespace NumMethods2.MatrixMath
{
    public class NumCore
    {
        public static List<double> FindMatrixSolutions(double[,] matrix, double[,] results,
            ref List<Tuple<double[,], double[,]>> iterationLog)
        {
            var output = new List<double>();
            var flatResults = results.Cast<double>().ToArray();
            var resLen = flatResults.Length;
            var X = new double[resLen];
            double sum = 0;
            for (var n = 0; n < resLen - 1; n++)
            {
                for (var i = n + 1; i < resLen; i++)
                {
                    var diagonal = matrix[n, n];
                    var rowFirst = matrix[i, n];
                    for (var j = n; j < resLen; j++)
                    {
                        if (diagonal == 0)
                        {
                            int rowToSwap;
                            matrix = SwapMatrixRows(matrix, n, resLen, out rowToSwap);
                            diagonal = matrix[n, n];
                            rowFirst = matrix[i, n];
                            flatResults = SwapResultsRows(flatResults, n, rowToSwap);
                        }
                        matrix[i, j] -= matrix[n, j]*rowFirst/diagonal;
                    }
                    flatResults[i] -= flatResults[n]*rowFirst/diagonal;
                }
                iterationLog?.Add(new Tuple<double[,], double[,]>((double[,]) matrix.Clone(),
                    flatResults.Select(flatResult => new List<double> {flatResult}).ToList().To2DArray()));
            }


            if (matrix[resLen - 1, resLen - 1] == 0)
            {
                if (flatResults[resLen - 1] == 0)
                    throw new InfiniteSystemSolutionsException();
                throw new NoSystemSolutionsException();
            }

            output.Add(X[resLen - 1] = flatResults[resLen - 1]/matrix[resLen - 1, resLen - 1]);

            for (var i = resLen - 2; i >= 0; i--)
            {
                for (var j = i + 1; j < resLen; j++)
                    sum += matrix[i, j]*X[j];

                output.Add(X[i] = (flatResults[i] - sum)/matrix[i, i]);
                sum = 0;
            }
            output.Reverse();

            return output;
        }

        public static double[] SwapResultsRows(double[] source, int rowFrom, int rowTo)
        {
            var elem = source[rowFrom];
            source[rowFrom] = source[rowTo];
            source[rowTo] = elem;
            return source;
        }

        public static double[,] SwapMatrixRows(double[,] source, int row, int size, out int swaps)
        {
            var list = new List<List<double>>();
            var flatMatrix = source.Cast<double>();
            for (var i = 0; i < size; i++)
            {
                var matrixRow = flatMatrix.Skip(i*size).Take(size).ToList();
                list.Add(matrixRow);
            }

            for (var i = 1; i < size - row; i++)
                if (list[row + i][row + i] != 0)
                {
                    swaps = row + i;
                    var listRow = list[row];
                    list.RemoveAt(row);
                    list.Insert(row + i, listRow);
                    return list.To2DArray();
                }


            throw new InfiniteSystemSolutionsException();
        }
    }
}