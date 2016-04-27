using System;
using NumMethods3.ViewModel;

namespace NumMethods3.NumCore
{
    internal class Function1 : IFunction , ISelectable
    {
        private static readonly int _id = 1;
        public int Id => _id;

        public double GetValue(double x)
        {
            return Math.Abs(x);
        }

        public string TextRepresentation => "|x|";
    }

    internal class Function2 : IFunction, ISelectable
    {
        private static readonly int _id = 2;
        public int Id => _id;

        public double GetValue(double x)
        {
            return Math.Sin(x);
        }

        public string TextRepresentation => "sin(x)";
    }

    internal class Function3 : IFunction, ISelectable
    {
        private static readonly int _id = 3;
        public int Id => _id;

        public double GetValue(double x)
        {
            return Math.Pow(4, x+1) - Math.Pow(5, x/2) - 10;
        }

        public string TextRepresentation => "4^(x+1)-5^(x/2)-10";
    }

    internal class Function4 : IFunction, ISelectable
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