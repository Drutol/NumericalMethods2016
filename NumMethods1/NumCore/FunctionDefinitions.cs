using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods1.NumCore
{
    class Function1 : IFunction
    {
        public double GetValue(double x)
        {
            return x*x;
        }

        public string TextRepresentation => "x^2";

    }

    class Function2 : IFunction
    {
        public double GetValue(double x)
        {
            return Math.Sin(x);
        }

        public string TextRepresentation => "sin(x)";
    }

    class Function3 : IFunction
    {
        public double GetValue(double x)
        {
            return Math.Sqrt(x < 0 ? 0 : x);
        }

        public string TextRepresentation => "sqrt(x)";
    }
}
