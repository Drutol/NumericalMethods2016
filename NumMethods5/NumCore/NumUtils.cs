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
        }

        public class ApproximationByPolynomialLevel : ApproximationCriterium
        {
            public ApproximationByPolynomialLevel(int level)
            {
                ApproximationMode = ApproximationModes.PolynomialLevel;
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
}
