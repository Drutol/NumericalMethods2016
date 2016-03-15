using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NumMethods1.ViewModels;

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
        /// Function root.
        /// </returns>
        public static FunctionRoot GetFunctionRootBi(IFunction source,GetFunctionRootBiArgs args)
        {
            int counter = 0;

            double val1 = source.GetValue(args.FromX),
                val2 = source.GetValue(args.ToX);
            
            double from = args.FromX, to = args.ToX;
            double mid = (from + to) / 2;

            if (val1 * val2 >0)
                throw new ArgumentException();
            double midVal = source.GetValue(mid);
            while (args.MaxIterations == -1 ? Math.Abs(midVal) >= args.Approx : counter <= args.MaxIterations)
            {
                mid = (from + to) / 2;

                midVal = source.GetValue(mid);

                if ((midVal < 0 && val1 < 0) || (midVal > 0 && val1 > 0))
                    from = mid;
                else
                    to = mid;
                counter++;
            }          
            return new FunctionRoot
            {
                X =mid,Y=midVal,
                Iterated = counter+1,Method_Used = "Bi"
            };
        }
        
        /// <summary>
        /// This method will find a root using regula falsi method
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static FunctionRoot GetFunctionRootFalsi(IFunction source, GetFunctionRootBiArgs args)
        {
            double a = args.FromX, b = args.ToX, approx= args.Approx;
            int iter = args.MaxIterations, counter = 0;
            
            double faVal = source.GetValue(a), fbVal = source.GetValue(b);
            double x=(a*fbVal-b*faVal)/(fbVal-faVal);
            double fxVal = source.GetValue(x);

            if (faVal*fbVal > 0)
                throw new ArgumentException();

            while (iter == -1 ? Math.Abs(fxVal)>=approx : counter <= iter)
            {
                if (faVal*fxVal < 0)
                {
                    b = x;
                    fbVal = fxVal;
                }
                else
                {
                    a = x;
                    faVal = fxVal;
                }
                x = (a * fbVal - b * faVal) / (fbVal - faVal);
                fxVal = source.GetValue(x);
                counter++;
            }

            return new FunctionRoot
            {
                X = x, Y = fxVal,
                Iterated = counter+1 , Method_Used = "Falsi"
            };
        }
    }
}
