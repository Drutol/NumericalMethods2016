using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods2.MatrixMath
{
    class NumCore
    {
        public static List<double> FindMatrixSolutions(double[,] matrix , double[,] results)
        {
            var output = new List<double>();
            var flatResults = results.Cast<double>().ToArray();
            var resLen = flatResults.Length;
            double sum = 0;
            for (int i = 0; i < resLen - 1; i++)
                for (int j = i + 1; j < resLen; j++)
                {
                    for (int k = i; k < resLen; k++)
                    {
                        if (matrix[i, i] == 0)
                            matrix = Utils.SwapRows(matrix, i, resLen);
                        matrix[j, k] = matrix[j, k] - (matrix[i, k] * matrix[j, i]) / matrix[i, i];
                    }
                    flatResults[j] -= (flatResults[i] * matrix[j, i]) / matrix[i, i];
                }

            for (int i = resLen - 1; i >= 0; i--)
            {
                for (int j = i + 1; j < resLen; j++)
                    sum += flatResults[j] * matrix[i, j];
                output.Add(flatResults[i] - sum);
            }

            return output;

        }
    }
}
