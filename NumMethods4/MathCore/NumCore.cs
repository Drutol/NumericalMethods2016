using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumMethods4.MathCore
{
    public class NumCore
    {
        public static void SimpsonsMethod(double a, double b, int wybor, double e)
        {
            int ilePrzedzialow = 1;
            double h;// = Math.abs(b-a)/2;
            double[] punkty = new double[1000000];
            double[] wartosciPunktow = new double[1000000];
            double suma1 = 0, suma2 = 0;

            do
            {
                int ilePunktow = 2 * ilePrzedzialow + 1;
                h = Math.abs(b - a) / (2 * ilePrzedzialow);     //odleglosc miedzy punktami dla danej ilosci przedzialow
                WypelnijTablicePunktow(punkty, a, b, h, ilePunktow);
                suma2 = suma1;
                suma1 = 0;
                for (int i = 0; i < (ilePunktow); i++)
                {
                    wartosciPunktow[i] = funkcja(wybor, punkty[i]);

                    if (i == 0 || i == ilePunktow - 1)
                    {
                        suma1 += wartosciPunktow[i];
                    }
                    else
                    {
                        if (i % 2 == 1)
                        {
                            suma1 += 4 * wartosciPunktow[i];
                        }
                        else
                        {
                            suma1 += 2 * wartosciPunktow[i];
                        }
                    }
                }

                suma1 *= (h / 3);       //wynik

                ilePrzedzialow *= 2;
                //System.out.println(suma1 + " " + suma2 + " " + h);
            } while (Math.abs(suma1 - suma2) > e);
            System.out.println("Calka funkcji " + nazwyFunkcji(wybor) + " na przedziale od " + a + " do " + b + " wynosi: " + suma1);
        }

        //public static void SimpsonsMethodPt2(int wybor, double e)
        //{
        //    double a = 0;
        //    double b = -1;
        //    double c = -1;
        //    int iterator = 1;
        //    double h;// = Math.abs(b-a)/2;
        //    double sumad1 = 0, sumad2 = 0;
        //    double sumau1 = 0, sumau2 = 0;
        //    do
        //    {
        //        if (iterator != 1)
        //        {
        //            a = b;
        //            b = b + c;
        //        }
        //        sumad1 = 0;
        //        sumau1 = 0;
        //        h = (a + b) / 2;        //odleglosc miedzy punktami dla danej ilosci przedzialow
        //        sumad1 = (Math.abs(b - a) / 3) * (waga(Math.abs(a)) * funkcja(wybor, Math.abs(a)) + 4 * waga(Math.abs(h)) * funkcja(wybor, Math.abs(h)) + waga(Math.abs(b)) * funkcja(wybor, Math.abs(b)));
        //        sumau1 = (Math.abs(b - a) / 3) * (waga(a) * funkcja(wybor, a) + 4 * waga(h) * funkcja(wybor, h) + waga(b) * funkcja(wybor, b));
        //        if (sumad1 > e)
        //        {
        //            sumad2 += sumad1;
        //        }
        //        if (sumau1 > e)
        //        {
        //            sumau2 += sumau1;
        //        }

        //        iterator += 1;
        //        //System.out.println(iterator-1 + " " + sumau1 + " " + sumau2 + " " + sumad1 + " " + sumad2 + " " + h + " " + a + " " + b);
        //    } while (Math.abs(sumad1) > e && Math.abs(sumau1) > e);
        //    System.out.println("Ta sama calka obliczona metoda Simpsona: ");
        //    if (sumad1 < e)
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
