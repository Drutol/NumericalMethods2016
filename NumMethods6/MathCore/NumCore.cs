using System;
using System.Collections;
using System.Collections.Generic;
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
        public delegate double SmallRkDelegate(double x, double y);

        static double sixth = 1.0 / 6.0;

        public static IEnumerable<double> rk4(double x, double[] y, double dx, List<SmallRkDelegate> f)
        {
            double halfdx = 0.5 * dx;
            var k1 = f.Select(fun => dx*fun(x, y.Sum())).ToList();
            var k2 = f.Select(fun => dx*fun(x + halfdx, y.Select((val, i) => val + halfdx*k1[i]).Sum())).ToList();
            var k3 = f.Select(fun => dx*fun(x + halfdx, y.Select((val, i) => val + halfdx*k2[i]).Sum())).ToList();
            var k4 = f.Select(fun => dx*fun(x + dx, y.Select((val, i) => val + halfdx*k3[i]).Sum())).ToList();

            return y.Select((val, i) => val + (k1[i] + 2*k2[i] + 2*k3[i] + k4[i])/6.0);

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

        public IEnumerable<DataPoint> Run(double[] args,List<RungeKutta.SmallRkDelegate> fun)
        {
            while (x < target)
            {
                var result = RungeKutta.rk4(x, args, dx, fun);
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
