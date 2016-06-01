using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NumMethods4Lib.MathCore
{
    public abstract class Function
    {
        private Func<double,double> _currentFunc;
        protected abstract double GetWeightValue(double x);
        protected abstract double GetNormalValue(double x);

        private void SetFunc(bool enable)
        {
            if (enable)
                _currentFunc = GetWeightValue;
            else
                _currentFunc = GetNormalValue;
        }

        public double GetValue(double x)
        {
            return _currentFunc.Invoke(x);
        }

        public bool EnableWeight
        {
            set { SetFunc(value); }
        }
    }

    public class Function1 : Function, IFunction
    {
        protected override double GetWeightValue(double x)
        {
            return Math.Exp(-1 * x) * Math.Abs(x);
        }

        protected override double GetNormalValue(double x)
        {
            return Math.Abs(x);
        }

        public string TextRepresentation => "|x|";
    }

    public class Function2 : Function, IFunction
    {
        protected override double GetWeightValue(double x)
        {
            return Math.Exp(-1*x) * Math.Cos(x);
        }

        protected override double GetNormalValue(double x)
        {
            return Math.Cos(x);
        }

        public string TextRepresentation => "cos(x)";
    }

    public class Function3 : Function, IFunction 
    {
        protected override double GetWeightValue(double x)
        {
            return Math.Exp(-1 * x) *(x * (x + 4) + 10);
        }

        protected override double GetNormalValue(double x)
        {
            return x * (x + 4) + 10;
        }

        public string TextRepresentation => "x^2+4x+10";
    }

    public class Function4 : Function, IFunction
    {
        protected override double GetWeightValue(double x)
        {
            return Math.Exp(-1 * x) * 1 - x;
        }

        private List<double> _coeffs = new List<double> {-1, 25, -200, 600, -600, 120};
        private double modifier = 1.0/120;
        protected override double GetNormalValue(double x)
        {
            return 1.0/120*(-1*x*x*x*x*x + 25*x*x*x*x - 200*x*x*x + 600*x*x - 600*x + 120);
        }

        public string TextRepresentation => "1-x";
    }

}
