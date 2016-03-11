using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods1.NumCore
{
    public interface IFunction
    {
        double GetValue(double x);
        string TextRepresentation { get; }
    }
}
