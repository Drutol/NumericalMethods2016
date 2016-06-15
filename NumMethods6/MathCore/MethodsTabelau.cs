using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods6.MathCore
{
    public class MethodsTabelau
    {
        public List<double> Nodes { get; set; }
        public List<double> Weights { get; set; }
        public double[,] RKMatrix { get; set; }
    }
}
