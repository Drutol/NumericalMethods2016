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
    public class Node
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    class NumCore
    {
        
    }

    public static class RungeKutta
    {
        private static double?[,] RK4 =
        {
            {0,0,0,0,0},
            {1.0/2,1.0/2,0,0,0 },
            {1.0/2,0,1.0/2,0,0 },
            {1.0,0,0,1.0,0 },
            {null,1.0/6,1.0/3,1.0/3,1.0/6 },
        };

        private static double?[,] Ralston =
        {
            {0,0,0},
            {2.0/3,2.0/3,0 },
            {null,1.0/4,3.0/4 }
        };

        public delegate double DiffFun(double t, List<double> variables);
        public delegate double DynamicDiffFun(double t, dynamic variables);

        //public static IEnumerable<double> Rk4(double x, double[] y, double step, List<DiffFun> f)
        //{
        //    double halfdx = 0.5*x;
        //    var k1 = f.Select(fun => step*fun(x, new List<double> {y[0], y[1], y[2], y[3]})).ToList();
        //    var k2 = f.Select(fun => step*fun(x + halfdx, new List<double>
        //    {
        //        y[0] + halfdx*k1[0], y[1] + halfdx*k1[1], y[2] + halfdx*k1[2], y[3] + halfdx*k1[3]
        //    })).ToList();
        //    var k3 = f.Select(fun => step*fun(x + halfdx, new List<double>
        //    {
        //        y[0] + halfdx*k2[0], y[1] + halfdx*k2[1], y[2] + halfdx*k2[2], y[3] + halfdx*k2[3]
        //    })).ToList();
        //    var k4 = f.Select(fun => step*fun(x + step, new List<double>
        //    {
        //        y[0] + step*k3[0], y[1] + step*k3[1], y[2] + step*k3[2], y[3] + step*k3[3]
        //    })).ToList();
        //    return y.Select((val, i) => val + (k1[i] + 2*k2[i] + 2*k3[i] + k4[i])/6.0);
        //}


        public static IEnumerable<double> Rk4(double x, double[] y, double step, List<DynamicDiffFun> f, List<string> parameters)
        {
            double halfdx = 0.5*step;
            var k1 = f.Select(fun => step*(double) fun(x, GetDynamicVariables(parameters, y))).ToList();
            var k2 = f.Select(fun => step*(double) fun(x + halfdx, GetDynamicVariables(parameters, y, k1, halfdx))).ToList();
            var k3 = f.Select(fun => step*(double) fun(x + halfdx, GetDynamicVariables(parameters, y, k2, halfdx))).ToList();
            var k4 = f.Select(fun => step*(double) fun(x + step, GetDynamicVariables(parameters, y, k3, step))).ToList();
            return y.Select((val, i) => val + (k1[i] + 2*k2[i] + 2*k3[i] + k4[i])/6.0);
        }

        private static dynamic GetDynamicVariables(List<string> parameters, double[] inits, List<double> kn, double dx)
        {
            var dyn = new ExpandoObject() as IDictionary<string, object>; //dynamic object Implements IDictionary
            for (int i = 0; i < parameters.Count; i++)
            {
                dyn.Add(parameters[i], inits[i] + dx*kn[i]); //thanks to implementation we can now call dyn.x for example
            }
            return dyn;
        }

        private static dynamic GetDynamicVariables(List<string> parameters, double[] inits)
        {
            var dyn = new ExpandoObject() as IDictionary<string, object>; //dynamic object Implements IDictionary
            for (int i = 0; i < parameters.Count; i++)
            {
                dyn.Add(parameters[i], inits[i]); //thanks to implementation we can now call dyn.x for example
            }
            return dyn;
        }
    }

    

    class RK4
    {
        public delegate double Calc(double t, double y);

        public double Runge(double t, double y, double dt, Calc yp)
        {
            double k1 = dt*yp(t, y);
            double k2 = dt*yp(t + 0.5*dt, y + k1*0.5*dt);
            double k3 = dt*yp(t + 0.5*dt, y + k2*0.5*dt);
            double k4 = dt*yp(t + dt, y + k3*dt);
            return (y + (1/6)*(k1 + 2*k2 + 2*k3 + k4));
        }
    }
}
