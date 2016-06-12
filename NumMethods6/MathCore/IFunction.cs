using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods6.MathCore
{
    public interface IFunction
    {
        string TextRepresentation { get; }
        
        double GetValue(double x, double y);
    }
}
