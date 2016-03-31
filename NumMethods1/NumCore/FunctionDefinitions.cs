using System;

namespace NumMethods1.NumCore
{
    internal class Function1 : IFunction
    {
        private static readonly int _id = 1;
        public int Id => _id;

        public double GetValue(double x)
        {
            return 4*x*Math.Sin(x*x)-Math.Pow(2,x);
        }

        public string TextRepresentation => "4*x*sin(x^2)-2^(x)";
    }

    internal class Function2 : IFunction
    {
        private static readonly int _id = 2;
        public int Id => _id;

        public double GetValue(double x)
        {
            return 15*Math.Sin(5*x) - 2*Math.Tan(x - 2);
        }

        public string TextRepresentation => "15*sin(5*x)-2*tan(x-2)";
    }

    internal class Function3 : IFunction
    {
        private static readonly int _id = 3;
        public int Id => _id;

        public double GetValue(double x)
        {
            return Math.Pow(4, x+1) - Math.Pow(5, x/2) - 10;
        }

        public string TextRepresentation => "4^(x+1)-5^(x/2)-10";
    }

    internal class Function4 : IFunction
    {
        private static readonly int _id = 4;
        public int Id => _id;

        public double GetValue(double x)
        {
            return x*x*(0.5 - 10*x*x) + 18;
        }

        public string TextRepresentation => "(x^2)/2 - (x^4)*10 + 18";
    }
}