using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using NumMethods4Lib.MathCore;
using OxyPlot;
using NumMethods5.NumCore;

namespace NumMethods5.NumCore
{
    public static class NumCore
    {
        private static LaguerrePolynomialProvider PolynomialProvider { get; } = new LaguerrePolynomialProvider();

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
            fun.EnableWeight = false;
            var approx = GetApproxPolynomial(fun, level);
            foreach (var t in nodes)
                t.Y = approx.GetValue(t.X);
            error = GetError(fun, level);
            return nodes;
        }

        public static Polynomial GetApproxPolynomial(IFunction fun, int level)
        {
            Polynomial approx =
                PolynomialProvider[0].Coefficients.Select(coeff => coeff*LaguerreIntegration(fun, 0)).ToPolynomial();
            for (int i = 1; i < level + 1; i++)
            {
                var temp =
                    PolynomialProvider[i].Coefficients.Select(baseCoeff => baseCoeff*LaguerreIntegration(fun, i-1))
                        .ToPolynomial();
                approx += temp;

            }
            return approx;
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
            double sum = 0;
            var poly = PolynomialProvider[k];         
            //waga * funkcja * wartość wielomianu         
            foreach (var laguerreNode in LaguerreNodes)
            {
                var temp = fun.GetValue(laguerreNode.Item1)*poly.GetValue(laguerreNode.Item1);
                sum += laguerreNode.Item2*temp;
            }           
            return sum;
        }

        public static double NewNewtonCotes(IFunction fun,int maxIter,int k)
        {           
            //double delta=.5;
            //var fromX = 0;
            //double calka = 0, s = 0;
            //fun.EnableWeight = true;
            //for (int i = 1; i < maxIter; i++)
            //{
            //    var x = fromX + i * delta;
            //    var poly = LaguerrePolynomial(k, x);
            //    s += fun.GetValue(x - delta/2)*poly;
            //    calka += fun.GetValue(x)*poly;
            //}
            
            //calka = (delta / 6) * (fun.GetValue(fromX) + 2 * calka + 4 * s) * LaguerrePolynomial(k, fromX);
            //fun.EnableWeight = false;
            return 1;
        }

        private static double Approximation(IFunction fun,double x,int level,bool cotes)
        {
            double sum = 0;
            if (!cotes)
                for (int i = 0; i <= level; i++)
                {
                    sum += LaguerreIntegration(fun, i)*PolynomialProvider[i].GetValue(x);
                }
            else
                for (int i = 0; i <= level; i++)
                {
                    sum += NewNewtonCotes(fun, 100, i)*PolynomialProvider[i].GetValue(x);
                }

            return sum;
        }

        public static double Silnia(int n)
        {
            int s = 1;
            for (int i = 1; i <= n; i++) s *= i;
            return s;
        }

        public static IEnumerable<double> GetPolynomialCoeffs(int n)
        {
            for (int i = 0; i <= n; i++)
            {
                var x = Silnia(n) / (Silnia(i) * Silnia(n - i)) * (Math.Pow(-1, i) / Silnia(i));
                yield return x;
            }
        }

        public static Polynomial ToPolynomial(this IEnumerable<double> coeffs)
        {
            return new Polynomial {Coefficients = coeffs.ToList()};
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
