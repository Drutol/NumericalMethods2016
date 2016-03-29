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
            return 15 * Math.Sin(5 * x) - 2 * Math.Tan(x - 2);
        }

        public string TextRepresentation => "15*sin(5*x)-2*tan(x-2)";
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
            return x * x * ( 1 / 2 - 10*x*x ) + 2000;
        }

        public string TextRepresentation => "(x^2)/2 - (x^4)*10 + 18";
    }
}