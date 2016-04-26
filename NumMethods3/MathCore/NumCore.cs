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

        public static InterpolationDataPack GetInterpolatedFunctionData(InterpolationDataPack data)
        {
            int nodeCount;
            double toX, fromX;
            double nodeDist;
            int precision = 20000;
            data.nodes = new List<FunctionValue>();
            data.interpolated = new List<FunctionValue>();
            data.interpolationResults = new List<FunctionValue>();
            if (data.DrawFromX > data.InterpolateFromX)
                data.DrawFromX = data.InterpolateFromX;
            if (data.DrawToX < data.InterpolateToX)
                data.DrawToX = data.InterpolateToX;
            if (data.SelectedFunction != null)
            {
                nodeCount = data.InterpolationNodesCount;
                toX = data.InterpolateToX;
                fromX = data.InterpolateFromX;
                nodeDist = Math.Abs((fromX - toX) / (nodeCount - 1));

                for (int i = 0; i < nodeCount; i++)
                {
                    data.nodes.Add(new FunctionValue
                    {
                        X = i == nodeCount - 1 ? toX : fromX + i * nodeDist,
                    });
                    data.nodes[i].Y = data.SelectedFunction.GetValue(data.nodes[i].X);
                }
            }
            else
            {
                var nodesInPreparation = data.FunctionValues;
                if (nodesInPreparation.Select(value => value.X).Distinct().ToList().Count != nodesInPreparation.Count)
                    throw new ArgumentException();

                data.nodes = nodesInPreparation.OrderBy(value => value.X).ToList();
                nodeCount = nodesInPreparation.Count;
                toX = data.nodes.Last().X;
                fromX = data.nodes.First().X;
                nodeDist = Math.Abs((fromX - toX) / (nodeCount - 1));
            }

            var progressives = ProgressiveSubs(nodeCount, data.nodes.Select(value => value.Y).ToList());
            double xDiff = data.DrawToX - data.DrawFromX;
            if (data.SelectedFunction != null)
                for (double i = 0; i < precision; i++)
                {
                    var current = new FunctionValue();
                    current.X = data.DrawFromX + i * (xDiff) / precision;
                    if (current.X > data.InterpolateFromX && current.X < data.InterpolateToX)
                    {
                        double t = (current.X - data.nodes[0].X)/nodeDist;
                        current.Y = NewtonsInterpolation(t, progressives);
                        data.interpolationResults.Add(current);
                    }
                    if (data.SelectedFunction != null)
                        data.interpolated.Add(new FunctionValue
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
                coeficients.Add(data.nodes[0].Y);
                for (int j = 1; j < nodes; j++)
                {
                    k = (data.nodes.Count - nodes + j);
                    for (int i = 1; i < nodes - (j - 1); i++, k++)
                    {
                        val2 = (data.nodes[k].Y - data.nodes[k - 1].Y) / (data.nodes[i + j - 1].X - data.nodes[i - 1].X);
                        data.nodes.Add(new FunctionValue { X = 0, Y = val2 });
                        if (i == 1)
                        {
                            coeficients.Add(val2);
                        }
                    }
                }
                xDiff = data.InterpolateToX - data.InterpolateFromX;
                for (double x = 0; x < precision; x++)
                {
                    List<double> dif = new List<double>();
                    val2 = coeficients[0];
                    var val1 = fromX + x * (xDiff) / precision;
                    dif.Add(val1 - data.nodes[0].X);

                    for (int i = 0; i < nodes - 1; i++)
                    {
                        dif.Add((val1 - data.nodes[i + 1].X) * (dif[i]));
                    }
                    for (int i = 0; i < coeficients.Count - 1; i++)
                    {
                        val2 = val2 + (coeficients[1 + i] * dif[i]);
                    }
                    data.interpolationResults.Add(new FunctionValue { X = val1, Y = val2 });
                }
            }
            return data;
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

   
