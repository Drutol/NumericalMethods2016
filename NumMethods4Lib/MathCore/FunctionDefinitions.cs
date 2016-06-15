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
            return Math.Exp(x);
        }

        public string TextRepresentation => "e^x";
    }
}
