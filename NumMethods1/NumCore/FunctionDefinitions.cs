using System;

namespace NumMethods1.NumCore
{
    internal class Function1 : IFunction
    {
        private static int _id = 1;
        public int Id => _id;

        public double GetValue(double x)
        {
            return x * x * ( x - 2 ) + 50;
        }

        public string TextRepresentation => "x^3-(x^2)*2 + 50";
    }

    internal class Function2 : IFunction
    {
        private static int _id = 2;
        public int Id => _id;
        public double GetValue(double x)
        {
            return Math.Sin(x);
        }

        public string TextRepresentation => "sin(x)";
    }

    internal class Function3 : IFunction
    {
        private static int _id = 3;
        public int Id => _id;
        public double GetValue(double x)
        {
            return Math.Sqrt(x < 0 ? 0 : x);
        }

        public string TextRepresentation => "sqrt(x)";
    }

    internal class Function4 : IFunction
    {
        private static int _id = 4;
        public int Id => _id;
        public double GetValue(double x)
        {
            return x * ( x * x / 2 - 10 ) + 18;
        }

        public string TextRepresentation => "(x^3)/2 - x*10 + 18";
    }
}