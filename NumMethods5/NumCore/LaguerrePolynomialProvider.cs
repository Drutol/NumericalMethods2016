using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace NumMethods5.NumCore
{
    public class LaguerrePolynomialProvider
    {
        private readonly Dictionary<int,Polynomial> _laguerrePolynomials = new Dictionary<int, Polynomial>();

        public Polynomial this[int i]
        {
            get
            {
                var newPoly = new Polynomial();
                if (_laguerrePolynomials.ContainsKey(i))
                {
                    
                    newPoly.Coefficients.AddRange(_laguerrePolynomials[i].Coefficients);
                    return newPoly;
                }

                var poly = BuildLaguerrePolynomial(i);
                _laguerrePolynomials.Add(i, poly);

                newPoly.Coefficients.AddRange(poly.Coefficients);
                return newPoly;
            }
        }

        private Polynomial BuildLaguerrePolynomial(int level)
        {
            var poly = new Polynomial();
            int start = 0;
            //if (_laguerrePolynomials.Count != 0)
            //{
            //    start = _laguerrePolynomials.Keys.Where(i => i < level).Max();
            //    poly.Coefficients = _laguerrePolynomials[start].Coefficients;
            //}
            for (int i = start; i <= level; i++)
                poly.Coefficients.Add(NumCore.Silnia(level)/
                                      (NumCore.Silnia(i)*NumCore.Silnia(level - i))*
                       (Math.Pow(-1, i)/NumCore.Silnia(i)));
            poly.Coefficients.Reverse();
            return poly;
        }

    }
}
