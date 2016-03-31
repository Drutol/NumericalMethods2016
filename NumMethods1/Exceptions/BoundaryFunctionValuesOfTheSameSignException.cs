using System;

namespace NumMethods1.Exceptions
{
    internal class BoundaryFunctionValuesOfTheSameSignException : Exception
    {
        public double LeftValue { get; set; }
        public double RightValue { get; set; }
    }
}