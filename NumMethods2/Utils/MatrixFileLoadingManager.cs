using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using NumMethods2.Exceptions;

namespace NumMethods2
{
    public static class MatrixFileLoadingManager
    {
        /// <summary>
        /// Deserializes files in xml,csv and json formats.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Tuple<double[,],double[,]> LoadData(string data)
        {
            data = data.Trim();
            if (data.Substring(0, 5) == "<?xml") //xml
            {
                var doc = XElement.Parse(data);
                var coeffs = new List<List<double>>();
                var results = new List<List<double>>();
                foreach (var eq in doc.Element("equations").Elements("equation"))
                {
                    coeffs.Add(new List<double>());
                    foreach (var coeff in eq.Element("coefficients").Elements("coeff"))
                        coeffs[coeffs.Count-1].Add(double.Parse(coeff.Value));
                    results.Add(new List<double> {double.Parse(eq.Element("result").Value)});
                }
                if(coeffs.Any( eq => eq.Count != coeffs[0].Count) || results.Count != coeffs[0].Count)
                    throw new InvalidMatrixFileException();

                return new Tuple<double[,], double[,]>(Utils.To2DArray(coeffs),Utils.To2DArray(results));
            }
            else if ((data.StartsWith("{") && data.EndsWith("}")) || (data.StartsWith("[") && data.EndsWith("]"))) 
            {
                return JsonConvert.DeserializeObject<Tuple<double[,], double[,]>>(data);
            }
            else //csv
            {
                var coeffs = new List<List<double>>();
                var results = new List<List<double>>();
                foreach (var line in data.Split(new char[] {'\r','\n'},StringSplitOptions.RemoveEmptyEntries))
                {
                    coeffs.Add(new List<double>());
                    foreach (var coeff in line.Split(','))
                    {
                        coeffs[coeffs.Count-1].Add(double.Parse(coeff.Replace('.',',')));
                    }
                    results.Add(new List<double> {coeffs[coeffs.Count-1].Last()});
                    coeffs[coeffs.Count - 1].RemoveAt(coeffs.Count-1);
                }

                if (coeffs.Any(eq => eq.Count != coeffs[0].Count) || results.Count != coeffs[0].Count)
                    throw new InvalidMatrixFileException();

                return new Tuple<double[,], double[,]>(Utils.To2DArray(coeffs), Utils.To2DArray(results));
            }

        }
    }
}
