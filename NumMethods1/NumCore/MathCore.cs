using System;

namespace NumMethods1.NumCore
{
    /// <summary>
    ///     Class containing methods responsible for finding function roots.
    /// </summary>
    public static class MathCore
    {
        /// <summary>
        ///     This method will find all roots of passed function using bisection.
        /// </summary>
        /// <param name="source">
        ///     Function definition that implements IFunction interface.
        /// </param>
        /// <param name="args">
        ///     Arguments class containging info like max iterations value , first & second endpoints
        ///     of the interval given.
        /// </param>
        /// <returns>
        ///     Function root and metadata.
        /// </returns>
        public static FunctionRoot GetFunctionRootBi(IFunction source, GetFunctionRootBiArgs args)
        {
            var counter = 0;

            double val1 = source.GetValue(args.FromX),
                val2 = source.GetValue(args.ToX);

            double from = args.FromX, to = args.ToX;
            var mid = (from + to)/2;

            if (val1*val2 > 0)
                throw new ArgumentException();
            var midVal = source.GetValue(mid);
            while (Math.Abs(midVal) >= args.Approx && counter < args.MaxIterations)
            {
                mid = (from + to)/2;

                midVal = source.GetValue(mid);

                if ((midVal < 0 && val1 < 0) || (midVal > 0 && val1 > 0))
                    from = mid;
                else
                    to = mid;
                counter++;
            }
            return new FunctionRoot
            {
                X = mid,
                Y = midVal,
                Iterated = counter,
                Method_Used = "Bi",
                Group = source.TextRepresentation
            };
        }

        /// <summary>
        ///     This method will find a root using regula falsi method
        /// </summary>
        /// <param name="source">
        ///     Function definition that implements IFunction interface.
        /// </param>
        /// <param name="args">
        ///     Arguments class containging info like max iterations value , first & second endpoints
        ///     of the interval given.
        /// </param>
        /// <returns>
        ///     Function root and metadata.
        /// </returns>
        public static FunctionRoot GetFunctionRootFalsi(IFunction source, GetFunctionRootBiArgs args)
        {
            int counter=0;
            double approx = args.Approx, iter = args.MaxIterations;
            double a = args.FromX, faVal = source.GetValue(args.FromX);
            double b = args.ToX, fbVal = source.GetValue(args.ToX);
            double x = a - (faVal / (fbVal - faVal)) * (b - a);
            double fxVal = source.GetValue(x);

            if(faVal*fbVal>0)
                throw new ArgumentException();
            
            while ((counter++ < iter) && (Math.Abs(fxVal) > approx))
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
                x = a - (faVal / (fbVal - faVal)) * (b - a);
                fxVal = source.GetValue(x);
            }

            return new FunctionRoot
            {
                X = x,
                Y = fxVal,
                Iterated = counter,
                Method_Used = "Falsi",
                Group = source.TextRepresentation
            };
        }
    }
}