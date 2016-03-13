using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods1.NumCore
{   
    /// <summary>
    /// Defines arguments passed to GetFunctionRootBi method.
    /// </summary>
    public class GetFunctionRootBiArgs
    {
        public int MaxIterations { get; set; }
        public double Approx { get; set; }
        public double FromX { get; set; }
        public double ToX { get; set; }
    }
}
