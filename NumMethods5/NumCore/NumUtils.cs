using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}