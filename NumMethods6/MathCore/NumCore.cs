using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using NumMethods6.ViewModel;
using OxyPlot;

namespace NumMethods6.MathCore
{
    public static class DifferentialService
    {
        //final
        private static double[] finalWeights = new double[] { 0.17476028, -0.55148053, 1.20553547, 0.17118478 };
        //step
        private static double[] stepWeights = { 0, 0.4, 0.45573726, 1 };
        //weight per k value
        private static double[] b2 = { 0.4 };                                               
        private static double[] b3 = { 0.29697760, 0.15875966 };                            
        private static double[] b4 = { 0.21810038, -3.05096470, 3.83286432 };				

        public delegate double DynamicDiffFun(double t, dynamic variables);

        public static IEnumerable<double> Rk4(double x, IEnumerable<double> y, double step, List<DynamicDiffFun> f, List<string> parameters)
        {
            double halfStep = 0.5 * step;
            var k1 = f.Select(fun => step * (double)fun(x, GetDynamicVariables(parameters, y))).ToList();
            var k2 = f.Select(fun => step * (double)fun(x + halfStep, GetDynamicVariables(parameters, y, k1, halfStep))).ToList();
            var k3 = f.Select(fun => step * (double)fun(x + halfStep, GetDynamicVariables(parameters, y, k2, halfStep))).ToList();
            var k4 = f.Select(fun => step * (double)fun(x + step, GetDynamicVariables(parameters, y, k3, step))).ToList();
            return y.Select((val, i) => val + (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]) / 6.0);
        }

        public static IEnumerable<double> Ralson(double x, IEnumerable<double> y, double step, List<DynamicDiffFun> f, List<string> parameters)
        {
            var k1 = f.Select(fun => step * (double)fun(x, GetDynamicVariables(parameters, y))).ToList();

            double weightedStep = stepWeights[1] * step;

            var k2 = f.Select(fun => step * (double)fun(x + weightedStep, GetDynamicVariables(parameters, y, k1,b2))).ToList();

            weightedStep = stepWeights[2] * step;

            var k3 = f.Select(fun => step * (double)fun(x + weightedStep, GetDynamicVariables(parameters, y, k2, b3))).ToList();
            var k4 = f.Select(fun => step * (double)fun(x + step, GetDynamicVariables(parameters, y, k3,b4))).ToList();
            return y.Select((val, i) => val + (k1[i]*finalWeights[0] + finalWeights[1] * k2[i] + finalWeights[2] * k3[i] + finalWeights[3] * k4[i]));
        }

        private static dynamic GetDynamicVariables(List<string> parameters, IEnumerable<double> inits, List<double> kn, double dx)
        {
            var dyn = new ExpandoObject() as IDictionary<string, object>; //dynamic object Implements IDictionary
            int i = 0;
            foreach (var init in inits.Zip(parameters, Tuple.Create))
                dyn.Add(init.Item2, init.Item1 + dx * kn[i++]);
            return dyn;
        }

        private static dynamic GetDynamicVariables(List<string> parameters, IEnumerable<double> inits, List<double> kn,double[] weights)
        {
            var dyn = new ExpandoObject() as IDictionary<string, object>; //dynamic object Implements IDictionary
           // int i = 0;
            foreach (var init in inits.Zip(parameters, Tuple.Create))
                dyn.Add(init.Item2, init.Item1 + kn.Take(weights.Length).Select((kj, i) => kj * weights[i]).Sum());
            return dyn;
        }

        private static dynamic GetDynamicVariables(List<string> parameters, IEnumerable<double> inits)
        {
            var dyn = new ExpandoObject() as IDictionary<string, object>;
            foreach (var init in inits.Zip(parameters, Tuple.Create))
                dyn.Add(init.Item2, init.Item1);
            return dyn;
        }
    }
}
