using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace NumMethods4Lib.MathCore
{
    public enum IntervalTypes
    {
        BothOpen,
        BothClosed,
        LeftOpen, //right closed
        RightOpen,
        InfLeftRightOpen,
        InfLeftRightClosed,
        InfRightLeftOpen,
        InfRightLeftClosed,
        InfBoth,
    }

    public struct Point
    {
        public double X;
        public double Y;
    }

    public static class NumCore
    {
        private static double NewtonikKotesik(double fromX, double toX, IFunction selecetedFunction, double acc, int maxIter, IntervalTypes type = IntervalTypes.BothClosed)
        {
            int intervals = 1, iter = 0;
            var nodes = new List<Point> { new Point { X = fromX, Y = selecetedFunction.GetValue(fromX) } };
            double sum1 = 0, sum2;
            double xFrom = fromX, xTo = toX;
            do
            {
                var nodeCount = 2 * intervals + 1;
                double delta;
                {
                    switch (type)
                    {
                        case IntervalTypes.BothOpen:
                            var dif = (xTo - xFrom) / (2 * intervals);
                            fromX = xFrom + dif;
                            toX = xTo - dif;
                            break;
                        case IntervalTypes.BothClosed:
                            break;
                        case IntervalTypes.LeftOpen:
                            fromX = xFrom + (xTo - xFrom) / (2 * intervals);
                            break;
                        case IntervalTypes.RightOpen:
                            toX = xTo - (xTo - xFrom) / (2 * intervals);
                            break;
                        default:
                            return InfiniteNewtonikCortesik(fromX, toX, selecetedFunction, maxIter, type);
                    }
                    delta = (toX - fromX) / (2 * intervals);
                    nodes.DoNodes(delta, nodeCount, selecetedFunction);
                }
                sum2 = sum1;
                sum1 = nodes.Skip(1).Take(nodeCount - 1).Select((point, i) => ((i + 1) % 2 == 1 ? 4 : 2) * point.Y).Sum() * delta / 3;
                intervals *= 2;
            } while (Math.Abs(sum1 - sum2) > acc && iter++ < maxIter);
            if (iter >= maxIter)
                throw new IndexOutOfRangeException("Iteration ammount exceded its limit.");
            return sum1;
        }

        private static double InfiniteNewtonikCortesik(double fromX, double toX, IFunction selecetedFunction, int maxIter, IntervalTypes type)
        {
            int intervals = 1, iter = 0;
            double node = fromX;
           
            double sum1 = 0;
            double delta = .5f;

            
            while (iter++ < maxIter)
            {
                sum1 += selecetedFunction.GetValue(node)*delta/3;
                if (type == IntervalTypes.InfLeftRightOpen || type == IntervalTypes.InfLeftRightClosed)
                    node -= delta;
                else
                    node += delta;
            }
            if (iter == maxIter)
                throw new IndexOutOfRangeException("Iteration ammount exceded its limit.");
            return sum1;
        }

        public static double NewNewtonCotes(double from, double to, int maxIter, IFunction fun, IntervalTypes type )
        {
            if (type != IntervalTypes.BothClosed)
                return NewtonikKotesik(from, to, fun, 0.01, maxIter, type); //nie potrafimy przystosować zoptymalizowanego algoytmu do przedziałów otwartych więc korzystamy z poprzedniego
            
            double delta, fromX=from, toX=to;
            bool inf=false;
            if (!double.IsPositiveInfinity(toX))
                delta = (toX - fromX)/maxIter;
            else
            {
                inf = true;
                fromX = 0;
                delta = .5;
            }
            double calka = 0, s = 0;
            for (int i = 1; i < maxIter; i++)
            {
                var x = fromX + i*delta;
                s += fun.GetValue(x - delta/2);
                calka += fun.GetValue(x);
            }
            if (inf)
                calka = (delta / 6) * (fun.GetValue(fromX) + 2 * calka + 4 * s);
            else if (type == IntervalTypes.BothClosed)
            {
                s += fun.GetValue(toX - delta/2);
                calka = (delta/6)*(fun.GetValue(fromX) + fun.GetValue(toX) + 2*calka + 4*s);
            }
            return calka;
        }

        /// <summary>
        /// Source: http://mathworld.wolfram.com/Laguerre-GaussQuadrature.html
        /// </summary>
        private static readonly List<Tuple<double, double>> LaguerreNodes = new List<Tuple<double, double>>
        {
            new Tuple<double, double>(0.26356, 0.521756), //root , weight
            new Tuple<double, double>(1.4134, 0.398667), new Tuple<double, double>(3.59643, 0.0759424), new Tuple<double, double>(7.08581, 0.00361176), new Tuple<double, double>(12.6408, 0.00002337),
        };

        public static double LaguerreIntegration(IFunction fun, int n)
        {
            return LaguerreNodes.Take(n).Sum(node => node.Item2*fun.GetValue(node.Item1));
        }

        private static void DoNodes(this List<Point> list, double xDiff, int target, IFunction fun)
        {
            for (int i = 1; i < target; i += 2)
            {
                var curr = list[i - 1].X + xDiff;
                list.Insert(i, new Point {X = curr, Y = fun.GetValue(curr)});
            }

            if (target == 3) //init whatnot
                list.Add(new Point {X = list.Last().X + xDiff, Y = fun.GetValue(list.Last().X + xDiff)});
        }
    }
}
