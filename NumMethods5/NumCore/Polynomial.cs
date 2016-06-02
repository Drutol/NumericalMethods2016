using System.Collections.Generic;
using System.Linq;

namespace NumMethods5.NumCore
{
    public class Polynomial
    {
        public List<double> Coefficients = new List<double>();

        public double GetValue(double x)
        {
            var result = Coefficients[0];
            foreach (var coeff in Coefficients.Skip(1))
            {
                result *= x;
                result += coeff;
            }
            return result;
        }
    }
}