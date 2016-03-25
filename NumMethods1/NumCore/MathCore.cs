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
        public static FunctionRoot GetFunctionRootBi(IFunction source, GetFunctionRootArgs args)
        {
            var counter = 0;

            double val1 = source.GetValue(args.FromX),
                val2 = source.GetValue(args.ToX);

            double from = args.FromX, to = args.ToX;
            var mid = (from + to)/2;

            if (Math.Abs(val1) < args.Approx)
            {
                
            }
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
                Group = source.TextRepresentation,
                SourceId = source.Id,
                Interval = $"[{args.FromX};{args.ToX}]"
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
        public static FunctionRoot GetFunctionRootFalsi(IFunction source, GetFunctionRootArgs args)
        {
            double x = 0, fxVal = 0;
            int counter, side = 0;
            double a = args.FromX, faVal = source.GetValue(args.FromX);
            double b = args.ToX, fbVal = source.GetValue(args.ToX);

            if (faVal*fbVal>0)
                throw new ArgumentException();

            for (counter = 0; counter < args.MaxIterations; counter++)
            {
               x = (faVal * b - fbVal * a) / (faVal - fbVal);
               fxVal = source.GetValue(x);
                if (Math.Abs(fxVal) < args.Approx)
                    break;
                if (fxVal * fbVal > 0)
                {
                    b = x;
                    fbVal = fxVal;
                    if (side == -1)
                        faVal /= 2;
                    side = -1;
                }
                else if (faVal * fxVal > 0)
                {
                    a = x;
                    faVal = fxVal;
                    if (side == +1)
                        fbVal /= 2;
                    side = +1;
                }
                else
                    break;
            }

            return new FunctionRoot
            {
                X = x,
                Y = fxVal,
                Iterated = counter,
                Method_Used = "Falsi",
                Group = source.TextRepresentation,
                SourceId = source.Id,
                Interval = $"[{args.FromX};{args.ToX}]"
            };
        }
    }
}