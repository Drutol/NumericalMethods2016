using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace NumMethods3.MathCore
{
    public class FunctionValue
    {
        public double X { get; set; }
        public double Y { get; set; }

        public DataPoint ToDataPoint => new DataPoint(X,Y);
    }
}
