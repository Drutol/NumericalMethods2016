using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods5.NumCore
{
    public class Polynomial
    {
        public List<double> Coefficients = new List<double>();

        public double GetValue(double x)
        {
            double result = Coefficients[0];
            foreach (var coeff in Coefficients.Skip(1))
            {
                result *= x;
                result += coeff;
            }

            return result;
        }
    }
}
