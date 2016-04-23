using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods3.MathCore
{
    public static class NumCore
    {
        private static readonly Dictionary<int,int> FactorialCache = new Dictionary<int, int>();  

        public static double NewtonsInterpolation(double t, List<double> diffSub)
        {
            return diffSub.Select((t1, i) => t1* PolynomialBase(i, t)/GetFactorial(i)).Sum();
        }

        private static double PolynomialBase(int k, double t)
        {
            if (k == 0) return 1;
            double output = 1;
            for (int i = 0; i < k; i++)
                output *= t - i;
            return output;
        }

        public static List<double> ProgressiveSubs(int n, List<double> y)
        {
            var output = new List<double>();
            for (int k = 0; k < n; k++)
            {
                output.Add(0);
                for (int i = 0; i <= k; i++)
                    output[k] += Math.Pow(-1, k - i)*NewtonsSymbol(k, i)*y[i];
            }
            return output;
        }

        private static double NewtonsSymbol(int n, int k)
        {
            return GetFactorial(n)/(GetFactorial(k)*GetFactorial(n - k));
        }

        private static double GetFactorial(int n)
        {
            int output;
            if (FactorialCache.TryGetValue(n, out output))
                return output;
            output = 1;
            if (n == 0) return 1;
            for (int i = 2; i <= n; i++)
                output *= i;
            FactorialCache.Add(n,output);
            return output;
        }
    }
}

   
