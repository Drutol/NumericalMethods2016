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
    }

    public class ApproximationByPolymonialLevel : ApproximationCriterium
    {
        public ApproximationByPolymonialLevel(int level)
        {
            ApproximationMode = ApproximationModes.PolymonialLevel;
            Level = level;
        }
        public int Level { get; private set; }
    }

    public class ApproximationByAccuracy : ApproximationCriterium
    {
        public ApproximationByAccuracy(double acc)
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

        public static IEnumerable<Node> GetApproximatedPlotDataPoints(IFunction fun, Interval interval,int nodeCount,ApproximationCriterium mode)
        {
            var nodes = new List<Node>();
            nodes.MakeNodes(interval,nodeCount);
            int level = 0;
            if (mode is ApproximationByPolymonialLevel)
                level = (mode as ApproximationByPolymonialLevel).Level;
            fun.EnableWeight = true;
            for (int i = 0; i < nodes.Count; i++)
                nodes[i].Y = Approximation(fun, nodes[i].X, level);

            return nodes;
        }

        private static double Approximation(IFunction fun,double x,int level)
        {
            double sum = 0;
            for (int i = 0; i <= level; i++)
            {
                var polynomial = LaguerrePolymonial(i, x);
                sum += (NumMethods4Lib.MathCore.NumCore.LaguerreIntegration(fun, 5, polynomial)/
                        Math.Pow(Math.PI/Math.Pow(2, i - 1),2))*polynomial;
            }
            return sum;
        }

        private static double silnia(int n)
        {
            int s = 1;
            for (int i = 1; i <= n; i++) s *= i;
            return s;
        }

        private static double LaguerrePolymonial(int n, double x)
        {
            double sum = 0;
            for (int i = 0; i <= n; i++)
                sum += (silnia(n)/(silnia(i)*silnia(n - i)))*(Math.Pow(-1, i)/silnia(i))*Math.Pow(x, i);
            return sum;
            //if (n == 0) return 1;
            //if (n == 1) return 1 - x;
            //return ((2*n + 1 - x)*LaguerrePolymonial(n - 1, x) - n*LaguerrePolymonial(n - 2, x))/n + 1;
        }

        private static void MakeNodes(this List<Node> list, Interval interval, int nodeCount)
        {
            var delta = Math.Abs(interval.From - interval.To)/nodeCount;
            for (double i = interval.From; i <= interval.To; i+= delta)
                list.Add(new Node {X=i});
        }
    }
}
