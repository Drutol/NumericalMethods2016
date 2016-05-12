using System;

namespace NumMethods4Lib.MathCore
{

    public class Function1 : IFunction
    {
        private static readonly int _id = 1;
        public int Id => _id;

        public double GetValue(double x)
        {
            return Math.Abs(x);
        }

        public string TextRepresentation => "|x|";
    }

    public class Function2 : IFunction
    {
        private static readonly int _id = 2;
        public int Id => _id;

        public double GetValue(double x)
        {
            return Math.Cos(x);
        }

        public string TextRepresentation => "cos(x)";
    }

    public class Function3 : IFunction
    {
        private static readonly int _id = 3;
        public int Id => _id;

        public double GetValue(double x)
        {
            return 1/x*x;
        }

        public string TextRepresentation => "1/x^2";
    }

    public class Function4 : IFunction
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
