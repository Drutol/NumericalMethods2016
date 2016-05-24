using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumMethods4Lib.MathCore;
using OxyPlot;

namespace NumMethods5.NumCoreApprox
{
    public class Interval
    {
        public double From { get; set; }
        public double To { get; set; }
    }

    public enum ApproximationModes
    {
        PolymonialLevel,
        Accuracy,
    }

    public class Node
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public abstract class ApproximationCriterium
    {
        public ApproximationModes ApproximationMode { get; set; }
        public bool UseCotes;
        protected ApproximationCriterium(bool cotes)
        {
            UseCotes = false;
        }
    }

    public class ApproximationByPolymonialLevel : ApproximationCriterium
    {
        public ApproximationByPolymonialLevel(int level,bool useCotes) : base(useCotes)
        {
            ApproximationMode = ApproximationModes.PolymonialLevel;
            Level = level;
        }
        public int Level { get; private set; }
    }

    public class ApproximationByAccuracy : ApproximationCriterium
    {
        public ApproximationByAccuracy(double acc,bool useCotes) : base(useCotes)
        {
            ApproximationMode = ApproximationModes.Accuracy;
            Accuracy = acc;
        }
        public double Accuracy { get; private set; }
    }

    public static class NumCore
    {
        public static IEnumerable<DataPoint> GetAccuratePlotDataPoints(IFunction fun, Interval interval)
        {
            fun.EnableWeight = false;
            for (double i = interval.From; i <= interval.To; i++)
                yield return new DataPoint(i,fun.GetValue(i));
        }

        public static IEnumerable<Node> GetApproximatedPlotDataPoints(IFunction fun, Interval interval,int nodeCount,ApproximationCriterium mode,out double error)
        {
            var nodes = new List<Node>();
            nodes.MakeNodes(interval,nodeCount);
            int level = 0;
            if (mode is ApproximationByPolymonialLevel)
                level = (mode as ApproximationByPolymonialLevel).Level;
            fun.EnableWeight = true;
            foreach (var t in nodes)
                t.Y = Approximation(fun, t.X, level,mode.UseCotes);
            error = GetError(fun, level);
            return nodes;
        }

        private static readonly List<Tuple<double, double>> LaguerreNodes = new List<Tuple<double, double>>
        {
            new Tuple<double, double>(0.26356, 0.521756), //root , weight
            new Tuple<double, double>(1.4134, 0.398667), new Tuple<double, double>(3.59643, 0.0759424), new Tuple<double, double>(7.08581, 0.00361176), new Tuple<double, double>(12.6408, 0.00002337),
        };

        private static double LaguerreIntegration(IFunction fun, int n, int k)
        {
            return LaguerreNodes.Take(n).Sum(node => node.Item2 * fun.GetValue(node.Item1) * LaguerrePolymonial(k, node.Item1));
        }

        public static double NewNewtonCotes(IFunction fun,int maxIter,int k)
        {           
            double delta=.5;
            var fromX = 0;
            double calka = 0, s = 0;
            for (int i = 1; i < maxIter; i++)
            {
                var x = fromX + i * delta;
                s += fun.GetValue(x - delta / 2) * LaguerrePolymonial(k, x);
                calka += fun.GetValue(x);
            }
            calka = (delta / 6) * (fun.GetValue(fromX) + 2 * calka + 4 * s);
            return calka;
        }

        private static double Approximation(IFunction fun,double x,int level,bool cotes)
        {
            double sum = 0;
            if (!cotes)
                for (int i = 0; i <= level; i++)
                {
                    sum += LaguerreIntegration(fun, 5, i) /
                            Math.Pow(Math.PI / Math.Pow(2, i - 1), 2)
                            * LaguerrePolymonial(i, x);
                }
            else
                for (int i = 0; i <= level; i++)
                {
                    sum += NewNewtonCotes(fun, 100, i) /
                            Math.Pow(Math.PI / Math.Pow(2, i - 1), 2)
                            * LaguerrePolymonial(i, x);
                }

            return sum;
        }

        private static double Silnia(int n)
        {
            int s = 1;
            for (int i = 1; i <= n; i++) s *= i;
            return s;
        }

        private static double LaguerrePolymonial(int n, double x)
        {
            double sum = 0;
            for (int i = 0; i <= n; i++)
                sum += (Silnia(n)/(Silnia(i)*Silnia(n - i)))*(Math.Pow(-1, i)/Silnia(i))*Math.Pow(x, i);
            return sum;
            //if (n == 0) return 1;
            //if (n == 1) return 1 - x;
            //return ((2*n + 1 - x)*LaguerrePolymonial(n - 1, x) - n*LaguerrePolymonial(n - 2, x))/n + 1;
        }

        public static IEnumerable<string> GetPolynomialCoeffs(int n)
        {
            for (int i = 0; i <= n; i++)
            {
                var x = Silnia(n)/(Silnia(i)*Silnia(n - i))*(Math.Pow(-1, i)/Silnia(i));
                yield return x>=0 ? $"+{x:N2}x^{n - i}" : $"{x:N2}x^{n - i}";
            }
        }

        private static double GetError(IFunction fun, int level)
        {
            double sum = 0;
            foreach (var laguerreNode in LaguerreNodes)
            {
                var approx = fun.GetValue(laguerreNode.Item1) - Approximation(fun, laguerreNode.Item1, level, false);
                sum += laguerreNode.Item2*approx*approx;
            }
            return Math.Sqrt(sum);
        }

        private static void MakeNodes(this List<Node> list, Interval interval, int nodeCount)
        {
            var delta = Math.Abs(interval.From - interval.To)/nodeCount;
            for (double i = interval.From; i <= interval.To; i+= delta)
                list.Add(new Node {X=i});
        }
    }
}
