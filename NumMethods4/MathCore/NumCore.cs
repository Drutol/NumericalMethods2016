using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods4.MathCore
{
    public class NumCore
    {
        public static double SimpsonsMethod(double fromX, double toX,
            IFunction selecetedFunction, double acc)
        {
            int intervals = 1;
            double[] nodes = new double[1000000];
            double[] nodesVals = new double[1000000];
            double sum1=0, sum2;
            do
            {
                var nodeCount = 2 * intervals + 1;
                var xDiff = Math.Abs(toX - fromX) / (2 * intervals);
                WypelnijTablicePunktow(nodes, fromX, xDiff, nodeCount);
                sum2 = sum1;
                sum1 = 0;
                for (int i = 0; i < (nodeCount); i++)
                {
                    nodesVals[i] = selecetedFunction.GetValue(nodes[i]);
                    if (i == 0 || i == nodeCount - 1)
                        sum1 += nodesVals[i];
                    else
                    {
                        if (i % 2 == 1)
                            sum1 += 4 * nodesVals[i];
                        else
                            sum1 += 2 * nodesVals[i];
                    }
                }
                sum1 *= (xDiff / 3);
                intervals *= 2;
            } while (Math.Abs(sum1 - sum2) > acc);
            return sum1;
        }

        private static void WypelnijTablicePunktow(double[] nodes, double fromX,
            double xDiff, int nodeCount)
        {
            for (int i = 0; i < nodeCount; i++)
            {
                if (i == 0)
                {
                    nodes[0] = fromX;
                }
                else
                {
                    nodes[i] = nodes[i - 1] + xDiff;
                }
            }
        }

        public static double ExcludingEndpointsIntegration(double fromX, double toX,
            bool isLeftEx, bool isRightEx, IFunction selectedFunction, double includedEndpointsValue)
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
