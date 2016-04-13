using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods2.MatrixMath
{
    class NumCore
    {
        private static int _rowNum;
        public static int RowNum { get; set; }

        public static List<double> FindMatrixSolutions(double[,] matrix , double[,] results)
        {
            var output = new List<double>();
            var flatResults = results.Cast<double>().ToArray();
            var resLen = flatResults.Length;
            double[] X=new double[resLen];
            double sum = 0;
            for (int n = 0; n < resLen - 1; n++)
            {
                
                for (int i = n + 1; i < resLen; i++)
                {
                    double diagonal = matrix[n, n];
                    double rowFirst = matrix[i, n];
                    for (int j = n; j < resLen; j++)
                    {
                        if (diagonal == 0)
                        {
                            matrix = Utils.SwapMatrixRows(matrix, n, resLen);
                            diagonal = matrix[n, n];
                            rowFirst = matrix[i, n];
                            flatResults = Utils.SwapResultsRows(flatResults, n,_rowNum);
                        }
                        matrix[i, j] -= (matrix[n, j] * rowFirst) / diagonal;
                    }
                    flatResults[i] -= (flatResults[n]*rowFirst)/diagonal;
                }
            }
            
            output.Add(X[resLen-1]=flatResults[resLen-1]/matrix[resLen-1,resLen-1]);

            for (int i = resLen -2; i >= 0; i--)
            {
                for (int j = i +1; j < resLen; j++)
                    sum += matrix[i, j]*X[j];

                output.Add(X[i]=(flatResults[i]-sum)/matrix[i,i]);
                sum = 0;
            }
            output.Reverse();

            return output;

        }
    }
}
