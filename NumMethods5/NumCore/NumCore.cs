using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
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
        PolynomialLevel,
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
            UseCotes = cotes;
        }
    }

    public class ApproximationByPolynomialLevel : ApproximationCriterium
    {
        public ApproximationByPolynomialLevel(int level,bool useCotes) : base(useCotes)
        {
            ApproximationMode = ApproximationModes.PolynomialLevel;
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
            if (mode is ApproximationByPolynomialLevel)
                level = (mode as ApproximationByPolynomialLevel).Level;
            fun.EnableWeight = true;
            foreach (var t in nodes)
                t.Y = Approximation(fun, t.X, level,mode.UseCotes);
            error = GetError(fun, level);
            return nodes;
        }

        private static readonly List<Tuple<double, double>> LaguerreNodes = new List<Tuple<double, double>>
        {
            new Tuple<double, double>(0.2635603197181409102031              , 0.5217556105828086524759), //root , weight
            new Tuple<double, double>(1.413403059106516792218               , 0.3986668110831759274541),
            new Tuple<double, double>(3.596425771040722081223               , 0.0759424496817075953877),
            new Tuple<double, double>(7.085810005858837556922               , 0.003611758679922048454461),
            new Tuple<double, double>(12.64080084427578265943               , 2.33699723857762278911E-5)
        };

        private static double LaguerreIntegration(IFunction fun, int k)
        {
            fun.EnableWeight = false;
            double sum = 0;
            for (int i = 0; i < 5; i++)
            {
                //waga * funkcja * wartość wielomianu
                sum += LaguerreNodes[i].Item2*fun.GetValue(LaguerreNodes[i].Item1)*
                       LaguerrePolynomial(k, LaguerreNodes[i].Item1);
            }
            //var sum = LaguerreNodes.Sum(node => node.Item2 * fun.GetValue(node.Item1) * LaguerrePolynomial(k, node.Item1));
            return sum;
        }

        public static double NewNewtonCotes(IFunction fun,int maxIter,int k)
        {           
            double delta=.5;
            var fromX = 0;
            double calka = 0, s = 0;
            for (int i = 1; i < maxIter; i++)
            {
                var x = fromX + i * delta;
                s += fun.GetValue(x - delta / 2) * LaguerrePolynomial(k, x);
                calka += fun.GetValue(x);
            }
            calka = (delta / 6) * (fun.GetValue(fromX) + 2 * calka + 4 * s);// * LaguerrePolynomial(k, fromX);
            return calka;
        }

        private static double Approximation(IFunction fun,double x,int level,bool cotes)
        {
            double sum = 0;
            if (!cotes)
                for (int i = 0; i <= level; i++)
                {
                    sum += LaguerreIntegration(fun, i) /
                            Math.Pow(Silnia(i),2)
                            * LaguerrePolynomial(i, x);
                }
            else
                for (int i = 0; i <= level; i++)
                {
                    sum += NewNewtonCotes(fun, 100, i) /
                           Math.Pow(Silnia(i+1), 2)
                            * LaguerrePolynomial(i, x);
                }

            return sum;
        }

        private static double Silnia(int n)
        {
            int s = 1;
            for (int i = 1; i <= n; i++) s *= i;
            return s;
        }

        public static double LaguerrePolynomial(int n, double x)
        {
            double sum = 0;
            for (int i = 0; i <= n; i++)
                sum += (Silnia(n) / (Silnia(i) * Silnia(n - i))) * (Math.Pow(-1, i) / Silnia(i)) * Math.Pow(x, i);
            return sum;
            //if (n == 0) return 1;
            //if (n == 1) return 1 - x;
            //return ((2 * n + 1 - x) * LaguerrePolynomial(n - 1, x) - n * LaguerrePolynomial(n - 2, x)) / n + 1;
        }

        public static IEnumerable<string> GetPolynomialCoeffs(int n)
        {
            for (int i = 0; i <= n; i++)
            {
                var x = Silnia(n)/(Silnia(i)*Silnia(n - i))*(Math.Pow(-1, i)/Silnia(i));
                yield return x >= 0 ? $"+{x:N2}x^{i}" : $"{x:N2}x^{i}";
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
