using System;

namespace NumMethods4.MathCore
{

        internal class Function1 : IFunction
        {
            private static readonly int _id = 1;
            public int Id => _id;

            public double GetValue(double x)
            {
                return Math.Abs(x);
            }

            public string TextRepresentation => "|x|";
        }

        internal class Function2 : IFunction
        {
            private static readonly int _id = 2;
            public int Id => _id;

            public double GetValue(double x)
            {
                return Math.Sin(x);
            }

            public string TextRepresentation => "sin(x)";
        }

        internal class Function3 : IFunction
        {
            private static readonly int _id = 3;
            public int Id => _id;

            public double GetValue(double x)
            {
                return x * x * x * Math.Sin(3 * x - 15) + x * x * x * 4 + x * (x + 4) * 4 + Math.Abs(x * x * 3 + 2) - 100;
            }

            public string TextRepresentation => "x^3*sin(3x - 15) + 4x^3 + 4x^2 + 16x + |3x^2 + 2| - 100";
        }

        internal class Function4 : IFunction
        {
            private static readonly int _id = 4;
            public int Id => _id;

            public double GetValue(double x)
            {
                return x * x * (0.5 - 10 * x * x) + 18;
            }

            public string TextRepresentation => "(x^2)/2 - (x^4)*10 + 18";
        }
    
}
