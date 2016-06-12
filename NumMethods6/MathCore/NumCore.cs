using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
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
        public delegate double DiffFun(double t, List<double> variables);
        public delegate double DynamicDiffFun(double t, dynamic variables);

        public static IEnumerable<double> Rk4(double x, double[] y, double dx, List<DiffFun> f)
        {
            double halfdx = 0.5 * x;
            var k1 = f.Select(fun => dx*fun(x,new List<double> { y[0],y[1],y[2],y[3]})).ToList();
            var k2 = f.Select(fun => dx*fun(x + halfdx,new List<double> { 
                y[0] + halfdx * k1[0],
                y[1] + halfdx * k1[1],
                y[2] + halfdx * k1[2],
                y[3] + halfdx * k1[3]})).ToList();
            var k3 = f.Select(fun => dx * fun(x + halfdx,new List<double> { 
                y[0] + halfdx * k2[0],
                y[1] + halfdx * k2[1],
                y[2] + halfdx * k2[2],
                y[3] + halfdx * k2[3]})).ToList();
            var k4 = f.Select(fun => dx * fun(x + dx, new List<double> {
                y[0] + dx * k3[0],
                y[1] + dx * k3[1],
                y[2] + dx * k3[2],
                y[3] + dx * k3[3]})).ToList();
            return y.Select((val, i) => val + (k1[i] + 2*k2[i] + 2*k3[i] + k4[i])/6.0);

        }


        public static IEnumerable<double> Rk4(double x, double[] y, double dx, List<DynamicDiffFun> f,
            List<string> parameters)
        {
            double halfdx = 0.5*x;
            

            var k0 = new List<double>();
            for (int i = 0; i < y.Length; i++)
                k0.Add(0);
            var k1 = f.Select(fun => dx *(double)fun(x, GetDynamicVariables(parameters,y))).ToList();
            //var k1 = f.Select(fun => dx*(double)fun(x + halfdx, GetDynamicVariables(parameters, y, k0, 0))).ToList();
            var k2 = f.Select(fun => dx*(double)fun(x + halfdx, GetDynamicVariables(parameters, y, k1, halfdx))).ToList();
            var k3 = f.Select(fun => dx*(double)fun(x + halfdx, GetDynamicVariables(parameters, y, k2, halfdx))).ToList();
            var k4 = f.Select(fun => dx*(double)fun(x + dx, GetDynamicVariables(parameters, y, k3, dx))).ToList();

            //var k3 = f.Select(fun => dx*fun(x + halfdx, y.Select((val, i) => val + halfdx*k2[i]).Sum())).ToList();
            //var k4 = f.Select(fun => dx*fun(x + dx, y.Select((val, i) => val + halfdx*k3[i]).Sum())).ToList();

            return y.Select((val, i) => val + (k1[i] + 2*k2[i] + 2*k3[i] + k4[i])/6.0);

        }

        private static dynamic GetDynamicVariables(List<string> parameters, double[] inits,List<double> kn,double dx)
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

    public class Equation
    {
        double x, y, dx, target;

        public Equation(double x, double y, double dx, double target)
        {
            this.x = x;
            this.y = y;
            this.dx = dx;
            this.target = target;
        }

        public IEnumerable<DataPoint> Run(double[] args,List<RungeKutta.DiffFun> fun)
        {
            while (x < target)
            {
                var result = RungeKutta.Rk4(x, args, dx, fun);
                x += dx;
                yield return new DataPoint(x,result.First());
            }
        }

        private double dy_dt(double t, double y)
        {
            return y;
        }
    }

    class RK4
    {
        public delegate double Calc(double t, double y);
        public double Runge(double t, double y, double dt, Calc yp)
        {
            double k1 = dt * yp(t, y);
            double k2 = dt * yp(t + 0.5 * dt, y + k1 * 0.5 * dt);
            double k3 = dt * yp(t + 0.5 * dt, y + k2 * 0.5 * dt);
            double k4 = dt * yp(t + dt, y + k3 * dt);
            return (y + (1 / 6) * (k1 + 2 * k2 + 2 * k3 + k4));
        }
    }

}
