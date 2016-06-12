using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods6.MathCore
{
    
        public class Function1 : IFunction
        {
            public string TextRepresentation => "y'=x*y";
            public double GetValue(double x, double y)
            {
                return x*y;
            }
        }

        public class Function2 : IFunction
        {
            public string TextRepresentation => "y'=3*x*y^2 - x + 6";
            public double GetValue(double x, double y)
            {
                return 3*x*y*y - x + 6;
            }
        }

    public class Function3 : IFunction
    {
        public string TextRepresentation => "y'=1/x * y*(y-1)";
        public double GetValue(double x, double y)
        {
            return 1 / x * y * (y - 1);
        }
    }
}
