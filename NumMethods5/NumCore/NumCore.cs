using System;
using System.Collections.Generic;
using System.Linq;
using NumMethods4Lib.MathCore;
using OxyPlot;

namespace NumMethods5.NumCore
{
    public static class NumCore
    {
        private static readonly List<Tuple<double, double>> LaguerreNodes = new List<Tuple<double, double>>
        {
            new Tuple<double, double>(0.2635603197181409102031, 0.5217556105828086524759), //root , weight
            new Tuple<double, double>(1.413403059106516792218, 0.3986668110831759274541),
            new Tuple<double, double>(3.596425771040722081223, 0.0759424496817075953877),
            new Tuple<double, double>(7.085810005858837556922, 0.003611758679922048454461),
            new Tuple<double, double>(12.64080084427578265943, 2.33699723857762278911E-5)
        };

        private static LaguerrePolynomialProvider PolynomialProvider { get; } = new LaguerrePolynomialProvider();

        public static IEnumerable<DataPoint> GetAccuratePlotDataPoints(IFunction fun, Interval interval)
        {
            fun.EnableWeight = false;
            for (var i = interval.From; i <= interval.To; i++)
                yield return new DataPoint(i, fun.GetValue(i));
        }

        public static IEnumerable<Node> GetApproximatedPlotDataPoints(IFunction fun, Interval interval, int nodeCount,
            ApproximationCriterium mode, out Polynomial approx)
        {
            var nodes = new List<Node>();
            nodes.MakeNodes(interval, nodeCount);

            var approximatingPolynomial = new Polynomial();
            if (mode is ApproximationByPolynomialLevel)
            {
                var level = ((ApproximationByPolynomialLevel) mode).Level;
                fun.EnableWeight = false;
                approximatingPolynomial = mode.UseCotes
                    ? GetApproxPolynomial(fun, level, NewNewtonCotes)
                    : GetApproxPolynomial(fun, level, LaguerreIntegration);
            }
            else
            {
                var i = 1;
                var eps = ((ApproximationByAccuracy) mode).Accuracy;
                var cache = new Dictionary<double, Polynomial>();
                Func<IFunction, int, double> integratorFunc;
                if (mode.UseCotes)
                    integratorFunc = NewNewtonCotes;
                else
                    integratorFunc = LaguerreIntegration;
                while (i++ < 20)
                {
                    approximatingPolynomial = GetApproxPolynomial(fun, i, integratorFunc);
                    var error = GetError(fun, approximatingPolynomial);
                    if (error <= eps)
                        break;
                    cache.Add(error, new Polynomial {Coefficients = approximatingPolynomial.Coefficients}); 
                }
                if (i == 21)
                    approximatingPolynomial = cache.OrderBy(x => x.Key).First().Value; //choose lowest error
            }

            approx = approximatingPolynomial;
            foreach (var t in nodes)
                t.Y = approx.GetValue(t.X);
            return nodes;
        }

        private static Polynomial GetApproxPolynomial(IFunction fun, int level,
            Func<IFunction, int, double> integratorFunc)
        {
            var approx =
                PolynomialProvider[0].Coefficients.Select(coeff => coeff*integratorFunc(fun, 0)).ToPolynomial();

            for (var i = 1; i < level + 1; i++)
            {
                var integral = LaguerreIntegration(fun, i);
                var temp = PolynomialProvider[i].Coefficients.Select(coeff => integral*coeff).ToPolynomial();
                for (var j = 0; approx.Coefficients.Count != temp.Coefficients.Count; j++)
                    approx.Coefficients.Insert(0, 0);
                approx.Coefficients = approx.Coefficients.Zip(temp.Coefficients, (x, y) => x + y).ToList();
            }
            return approx;
        }

        private static double LaguerreIntegration(IFunction fun, int k)
        {
            double sum = 0;
            var poly = PolynomialProvider[k];
            //waga * funkcja * wartość wielomianu         
            foreach (var laguerreNode in LaguerreNodes)
                sum += laguerreNode.Item2* fun.GetValue(laguerreNode.Item1) * poly.GetValue(laguerreNode.Item1);
            return sum;
        }

        private static double NewNewtonCotes(IFunction fun, int k)
        {
            var poly = PolynomialProvider[k];
            fun.EnableWeight = true;
            double fromX = 0, calka = 0, s = 0, delta = .5;
            for (var i = 1; i < 100; i++)
            {
                var x = fromX + i*delta;
                s += fun.GetValue(x - delta/2)*poly.GetValue(x);
                calka += fun.GetValue(x)*poly.GetValue(x);
            }
            calka = delta/6*(fun.GetValue(fromX)*poly.GetValue(fromX) + 2*calka + 4*s);
            fun.EnableWeight = false;
            return calka;
        }

        public static double Factorial(int n)
        {
            var s = 1;
            for (var i = 1; i <= n; i++) s *= i;
            return s;
        }

        private static Polynomial ToPolynomial(this IEnumerable<double> coeffs)
        {
            return new Polynomial {Coefficients = coeffs.ToList()};
        }

        public static double GetError(IFunction fun, Polynomial poly)
        {
            double sum = (from laguerreNode in LaguerreNodes
                let approx = fun.GetValue(laguerreNode.Item1) - poly.GetValue(laguerreNode.Item1)
                select laguerreNode.Item2*approx*approx).Sum();
            return Math.Sqrt(sum);
        }

        private static void MakeNodes(this List<Node> list, Interval interval, int nodeCount)
        {
            var delta = Math.Abs(interval.From - interval.To)/nodeCount;
            for (var i = interval.From; i <= interval.To; i += delta)
                list.Add(new Node {X = i});
        }
    }
}