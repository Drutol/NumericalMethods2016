using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumMethods3.ViewModel;

namespace NumMethods3.MathCore
{
    public static class NumCore
    {
        private static readonly Dictionary<int,int> FactorialCache = new Dictionary<int, int>();

        public static void GetInterpolatedFunctionData(ref InterpolationDataPack data)
        {
            int nodeCount;
            double toX, fromX;
            double nodeDist = 0;
            int precision = 15000;
            data.Nodes = new List<FunctionValue>();
            data.Interpolated = new List<FunctionValue>();
            data.InterpolationResults = new List<FunctionValue>();
            
            if (data.SelectedFunction != null)
            {
                nodeCount = data.InterpolationNodesCount;
                toX = data.InterpolateToX;
                fromX = data.InterpolateFromX;
                nodeDist = Math.Abs((fromX - toX) / (nodeCount - 1));

                for (int i = 0; i < nodeCount; i++)
                {
                    data.Nodes.Add(new FunctionValue
                    {
                        X = i == nodeCount - 1 ? toX : fromX + i * nodeDist,
                    });
                    data.Nodes[i].Y = data.SelectedFunction.GetValue(data.Nodes[i].X);
                }
            }
            else
            {
                var nodesInPreparation = data.FunctionValues;
                if (nodesInPreparation.Select(value => value.X).Distinct().ToList().Count != nodesInPreparation.Count)
                    throw new ArgumentException();

                data.Nodes = nodesInPreparation.OrderBy(value => value.X).ToList();
                data.DrawToX = data.Nodes.Last().X;
                nodeCount = nodesInPreparation.Count;
                data.DrawFromX = data.Nodes.First().X;
            }

            var progressives = ProgressiveSubs(nodeCount, data.Nodes.Select(value => value.Y).ToList());
            double xDiff = data.DrawToX - data.DrawFromX;
            if (data.SelectedFunction != null)
                for (double i = 0; i < precision; i++)
                {
                    var current = new FunctionValue();
                    current.X = data.DrawFromX + i * (xDiff) / precision;
                    if (current.X > data.InterpolateFromX && current.X < data.InterpolateToX)
                    {
                        double t = (current.X - data.Nodes[0].X)/nodeDist;
                        current.Y = NewtonsInterpolation(t, progressives);
                        data.InterpolationResults.Add(current);
                    }
                    if (data.SelectedFunction != null)
                        data.Interpolated.Add(new FunctionValue
                        {
                            X = current.X,
                            Y = data.SelectedFunction.GetValue(current.X)
                        });
                }
            else
            {
                List<double> coeficients = new List<double>();
                int k;
                int nodes = nodeCount;
                double val2;
                coeficients.Add(data.Nodes[0].Y);
                for (int j = 1; j < nodes; j++)
                {
                    k = data.Nodes.Count - nodes + j;
                    for (int i = 1; i < nodes - (j - 1); i++, k++)
                    {
                        val2 = (data.Nodes[k].Y - data.Nodes[k - 1].Y) / (data.Nodes[i + j - 1].X - data.Nodes[i - 1].X);
                        data.Nodes.Add(new FunctionValue { X = 0, Y = val2 });
                        if (i == 1)
                            coeficients.Add(val2);
                    }
                }

                
                for (double x = 0; x < precision; x++)
                {
                    List<double> dif = new List<double>();
                    val2 = coeficients[0];
                    var val1 = data.DrawFromX + x * (xDiff) / precision;
                    dif.Add(val1 - data.Nodes[0].X);

                    for (int i = 0; i < nodes - 1; i++)
                        dif.Add((val1 - data.Nodes[i + 1].X) * dif[i]);
                    for (int i = 0; i < coeficients.Count - 1; i++)
                        val2 = val2 + coeficients[1 + i] * dif[i];
                    data.InterpolationResults.Add(new FunctionValue { X = val1, Y = val2 });
                }
            }
        }

        private static double NewtonsInterpolation(double t, List<double> diffSub)
        {
            return diffSub.Select((t1, i) => t1* PolynomialBase(i, t)/GetFactorial(i)).Sum();
        }

        private static double PolynomialBase(int k, double t)
        {
            if (k == 0) return 1;
            double output = 1;
            for (int i = 0; i < k; i++)
                output *= t - i;
            return output;
        }

        private static List<double> ProgressiveSubs(int n, List<double> y)
        {
            var output = new List<double>();
            for (int k = 0; k < n; k++)
            {
                output.Add(0);
                for (int i = 0; i <= k; i++)
                    output[k] += Math.Pow(-1, k - i)*NewtonsSymbol(k, i)*y[i];
            }
            return output;
        }

        private static double NewtonsSymbol(int n, int k)
        {
            return GetFactorial(n)/(GetFactorial(k)*GetFactorial(n - k));
        }

        private static double GetFactorial(int n)
        {
            int output;
            if (FactorialCache.TryGetValue(n, out output))
                return output;
            output = 1;
            if (n == 0) return 1;
            for (int i = 2; i <= n; i++)
                output *= i;
            FactorialCache.Add(n,output);
            return output;
        }
    }
}

   
