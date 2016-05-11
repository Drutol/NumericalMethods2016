using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods4.MathCore
{
    public enum IntervalTypes
    {
        BothOpen,
        BothClosed,
        LeftOpen, //right closed
        RightOpen,
        InfLeftRightOpen,
        InfLeftRightClosed,
        InfRightLeftOpen,
        InfRightLeftClosed,
        InfBoth,
    }

    public static class NumCore
    {
        public static double SimpsonsMethod(double fromX, double toX,IFunction selecetedFunction, double acc,IntervalTypes type = IntervalTypes.BothClosed)
        {
            int intervals = 1;
            var nodes = new List<double> {fromX};
            var nodesVals = new Dictionary<double,double>();
            double sum1=0, sum2;
            do
            {
                var nodeCount = 2 * intervals + 1;
                switch (type)
                {
                    case IntervalTypes.BothOpen:
                        var dif = (toX - fromX) / (2 * intervals);
                        fromX += dif;
                        toX -= dif;
                        break;
                    case IntervalTypes.BothClosed:
                        break;
                    case IntervalTypes.LeftOpen:
                        fromX += (toX - fromX) / (2 * intervals);
                        break;
                    case IntervalTypes.RightOpen:
                        toX -= (toX - fromX) / (2 * intervals);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
                var xDiff = Math.Abs(toX - fromX) / (2 * intervals);
                nodes.DoNodes(xDiff, nodeCount);
                sum2 = sum1;
                sum1 = selecetedFunction.GetValue(nodes[0]);               
                for (int i = 1; i < nodeCount-1; i++)
                {
                    double val;
                    if (!nodesVals.TryGetValue(nodes[i],out val))
                    {
                        val = selecetedFunction.GetValue(nodes[i]);
                        nodesVals.Add(nodes[i], val);
                    }                   
                    sum1 += (i%2 == 1 ? 4 : 2)*val;
                }
                sum1 += selecetedFunction.GetValue(nodes.Last());
                sum1 *= xDiff / 3;
                intervals *= 2;
            } while (Math.Abs(sum1 - sum2) > acc);
            return sum1;
        }

        private static void DoNodes(this List<double> list, double xDiff, int target)
        {
            for (int i=1;i<target;i+=2)
                list.Insert(i,list[i-1]+xDiff);
            if (target == 3) //init whatnot
                list.Add(list.Last() + xDiff);
        }


        public static double ExcludingEndpointsIntegration(double fromX, double toX, bool isLeftEx, bool isRightEx, IFunction selectedFunction, double includedEndpointsValue)
        {
            if (isLeftEx)
                includedEndpointsValue -= selectedFunction.GetValue(fromX);
            if (isRightEx)
                includedEndpointsValue -= selectedFunction.GetValue(toX);

            return includedEndpointsValue;
        }

        //public static void SimpsonsMethodPt2(int wybor, double acc)
        //{
        //    double fromX = 0;
        //    double toX = -1;
        //    double c = -1;
        //    int iterator = 1;
        //    double h;// = Math.abs(toX-fromX)/2;
        //    double sumad1 = 0, sumad2 = 0;
        //    double sumau1 = 0, sumau2 = 0;
        //    do
        //    {
        //        if (iterator != 1)
        //        {
        //            fromX = toX;
        //            toX = toX + c;
        //        }
        //        sumad1 = 0;
        //        sumau1 = 0;
        //        h = (fromX + toX) / 2;        //odleglosc miedzy punktami dla danej ilosci przedzialow
        //        sumad1 = (Math.abs(toX - fromX) / 3) * (waga(Math.abs(fromX)) * funkcja(wybor, Math.abs(fromX)) + 4 * waga(Math.abs(h)) * funkcja(wybor, Math.abs(h)) + waga(Math.abs(toX)) * funkcja(wybor, Math.abs(toX)));
        //        sumau1 = (Math.abs(toX - fromX) / 3) * (waga(fromX) * funkcja(wybor, fromX) + 4 * waga(h) * funkcja(wybor, h) + waga(toX) * funkcja(wybor, toX));
        //        if (sumad1 > acc)
        //        {
        //            sumad2 += sumad1;
        //        }
        //        if (sumau1 > acc)
        //        {
        //            sumau2 += sumau1;
        //        }

        //        iterator += 1;
        //        //System.out.println(iterator-1 + " " + sumau1 + " " + sumau2 + " " + sumad1 + " " + sumad2 + " " + h + " " + fromX + " " + toX);
        //    } while (Math.abs(sumad1) > acc && Math.abs(sumau1) > acc);
        //    System.out.println("Ta sama calka obliczona metoda Simpsona: ");
        //    if (sumad1 < acc)
        //    {
        //        System.out.println(sumau2);
        //    }
        //    else
        //    {
        //        System.out.println(sumad2);
        //    }
        //}
    }
}
