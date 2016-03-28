using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods1.Exceptions
{
    class BoundaryFunctionValuesOfTheSameSignException : Exception
    {
        public double LeftValue { get; set; }
        public double RightValue { get; set; }
    }
}
