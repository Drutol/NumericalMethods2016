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

        public static Polynomial operator +(Polynomial lewy, Polynomial prawy)
        {
            int size = lewy.Coefficients.Count;
            if (prawy.Coefficients.Count > size)
                size = prawy.Coefficients.Count;
            Polynomial nowy = new Polynomial();
            for(int i=0;i<size;i++)
                nowy.Coefficients.Add(0);
            // dopisujemy wspolczynniki lewego
            for (int i = 0; i < lewy.Coefficients.Count; i++)
            {
                nowy.Coefficients[size - 1 - i] += lewy.Coefficients[lewy.Coefficients.Count - 1 - i];
            }
            // dopisujemy wspolczynniki prawego
            for (int i = 0; i < prawy.Coefficients.Count; i++)
            {
                nowy.Coefficients[size - 1 - i] += prawy.Coefficients[prawy.Coefficients.Count - 1 - i];
            }
            return nowy;
        }
    }
}
