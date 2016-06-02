using System;
using System.Collections.Generic;
using NumMethods4Lib.MathCore;

namespace NumMethods5.NumCore
{
    public class Interval
    {
        public double From { get; set; }
        public double To { get; set; }
    }

    public enum ApproximationModes
    {
        PolynomialLevel,
        Accuracy
    }

    public class Node
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public abstract class ApproximationCriterium
    {
        public bool UseCotes;

        protected ApproximationCriterium(bool cotes)
        {
            UseCotes = cotes;
        }

        public ApproximationModes ApproximationMode { get; set; }
    }

    public class ApproximationByPolynomialLevel : ApproximationCriterium
    {
        public ApproximationByPolynomialLevel(int level, bool useCotes) : base(useCotes)
        {
            ApproximationMode = ApproximationModes.PolynomialLevel;
            Level = level;
        }

        public int Level { get; private set; }
    }

    public class ApproximationByAccuracy : ApproximationCriterium
    {
        public ApproximationByAccuracy(double acc, bool useCotes) : base(useCotes)
        {
            ApproximationMode = ApproximationModes.Accuracy;
            Accuracy = acc;
        }

        public double Accuracy { get; private set; }
    }

    public class Function4 : Function, IFunction
    {
        private readonly Polynomial _poly = new Polynomial
        {
            Coefficients = new List<double> {-1, 25, -200, 600, -600, 120}
        };

        public string TextRepresentation => "(1/120)*(-x⁵+25x⁴-200x³+600x²-600x+120)";

        protected override double GetWeightValue(double x)
        {
            return Math.Exp(-1*x)*1 - x;
        }

        protected override double GetNormalValue(double x)
        {
            return 1.0/120*_poly.GetValue(x);
        }
    }
}