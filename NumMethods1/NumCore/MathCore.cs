using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods1.NumCore
{
    public static class MathCore
    {
        /// <summary>
        /// This method will find all roots of passed function using bisection.
        /// </summary>
        /// <param name="source">
        /// Function definition that implements IFunction interface.
        /// </param>
        /// <param name="args">
        /// Arguments class containging info like max iterations value , start and and X coordinate.
        /// </param>
        /// <returns>
        /// List of points where passed function's value equals 0.
        /// </returns>
        public static double GetFunctionRootBi(IFunction source,GetFunctionRootBiArgs args)
        {
            double mid = 0;
            int counter = 0;

            double val1 = source.GetValue(args.FromX),
                val2 = source.GetValue(args.ToX);

            double from = args.FromX, to = args.ToX;

            if (val1 * val2 >0)
                throw new ArgumentException();

            while (counter++ <= args.MaxIterations)
            {
                mid = (from + to) / 2;

                var midVal = source.GetValue(mid);

                if ((midVal < 0 && val1 < 0) || (midVal > 0 && val1 > 0))
                    from = mid;
                else
                    to = mid;
            }          
            return mid;
        }

        public static double GetFunctionRootFalsi(IFunction source, GetFunctionRootBiArgs args)
        {
            double a = args.FromX, b = args.ToX, approx= args.Approx;
            int iter = args.MaxIterations;

            int counter = 0;
            double x;

            while (counter < iter)
            { 
                
                counter++;
            }

            return 0;
        }
    }
}
