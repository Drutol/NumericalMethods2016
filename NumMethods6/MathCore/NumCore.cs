using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumMethods4Lib.MathCore;
using NumMethods6.ViewModel;

namespace NumMethods6.MathCore
{
    class NumCore
    {
        public class Node
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        private MethodsTabelau RK4 = new MethodsTabelau
        {
            Nodes = {.0,.5,.5,1.0 },
            RKMatrix = new double[4,4]
            {
                { 0,0,0,0},
                { 1.0/2.0,0,0,0 },
                { 0,1.0/2.0,0,0 },
                { 0,0,1.0,0 }
            },
            Weights = { 1.0 / 3.0, 1.0 / 6.0, 1.0 / 6.0, 1.0 / 3.0 }
        };

        private MethodsTabelau Ralston = new MethodsTabelau
        {
            Nodes = { 0, 2.0 / 3.0 },
            RKMatrix = new double[2,2]
            {
                { 0,0},
                { 2.0/3.0,0}
            },
            Weights = {1.0/4.0,3.0/4.0}
        };

        private double GetIncrementY(IFunction source, double x, double y, double h, int range, MethodsTabelau method)
        {
            return y+h*(method.RKMatrix[range,]);
        }


        public List<Node> RungeKuty(Interval interval,int precision,MethodsTabelau method, IFunction fun, double y0, double x0, double h)
        {
            List<Node> result = new List<Node>();

            double step = Math.Abs(interval.To - interval.From)/precision;

            for (double i = interval.From; i <= interval.To; i += step)
                result.Add(new Node {X=i});

            result[0].Y = y0;

            for (int i = 1; i < result.Count; i++)
            {
                result[i].Y = result[i - 1].Y + h*1;
            }
            var sum = 0.0;
            for (int i = 0; i < result.Count; i++)
            {
                for (int j = 0; j < 0; j++)
                {
                    sum = fun.GetValue(result[i].X+method.Nodes[i]*h, result[i].Y+method.RKMatrix[i,j]*);
                }
            }




            return result;
        }
    }
}
