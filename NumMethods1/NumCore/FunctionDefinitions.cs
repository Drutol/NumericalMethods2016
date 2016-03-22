using System;

namespace NumMethods1.NumCore
{
    internal class Function1 : IFunction
    {
        public double GetValue(double x)
        {
            return Math.Pow(x, 3) - Math.Pow(x, 2)*2 + 50;
        }

        public string TextRepresentation => "x^3-(x^2)*2 + 50";
    }

    internal class Function2 : IFunction
    {
        public double GetValue(double x)
        {
            return Math.Sin(x);
        }

        public string TextRepresentation => "sin(x)";
    }

    internal class Function3 : IFunction
    {
        public double GetValue(double x)
        {
            return Math.Sqrt(x < 0 ? 0 : x);
        }

        public string TextRepresentation => "sqrt(x)";
    }

    internal class Function4 : IFunction
    {
        public double GetValue(double x)
        {
            return Math.Pow(x, 3)/2 - 10*x + 18;
        }

        public string TextRepresentation => "(x^3)/2 - x*10 + 18";
    }
}