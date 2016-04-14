using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using NumMethods2.Exceptions;

namespace NumMethods2
{
    public static class MatrixFileLoadingManager
    {
        /// <summary>
        ///     Deserializes files in xml,csv and json formats.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Tuple<double[,], double[,]> LoadData(string data)
        {
            data = data.Trim();
            if (data.Substring(0, 5) == "<?xml") //xml
            {
                var doc = XElement.Parse(data);
                var coeffs = new List<List<double>>();
                var results = new List<List<double>>();
                int? len = null;
                foreach (var eq in doc.Element("equations").Elements("equation"))
                {
                    coeffs.Add(new List<double>());
                    foreach (var coeff in eq.Element("coefficients").Elements("coeff"))
                        coeffs[coeffs.Count - 1].Add(double.Parse(coeff.Value));
                    //check if matrix is NxN
                    if (len == null)
                        len = coeffs[0].Count;
                    else if (len.Value != coeffs.Last().Count || len < coeffs.Count)
                        throw new InvalidMatrixFileException();

                    results.Add(new List<double> {double.Parse(eq.Element("result").Value)});
                }

                return new Tuple<double[,], double[,]>(coeffs.To2DArray(), results.To2DArray());
            }
            else if ((data.StartsWith("{") && data.EndsWith("}")) || (data.StartsWith("[") && data.EndsWith("]")))
            {
                return JsonConvert.DeserializeObject<Tuple<double[,], double[,]>>(data);
            }
            else 
            {
                var coeffs = new List<List<double>>();
                var results = new List<List<double>>();
                int? len = null;
                foreach (var line in data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    coeffs.Add(new List<double>());
                    foreach (var coeff in line.Split(','))
                    {
                        coeffs.Last().Add(double.Parse(coeff.Replace('.', ',')));
                    }
                    //check if matrix is NxN
                    if (len == null)
                        len = coeffs[0].Count;
                    else if (len.Value != coeffs.Last().Count || len < coeffs.Count)
                        throw new InvalidMatrixFileException();

                    results.Add(new List<double> { coeffs.Last().Last() });
                    coeffs.Last().RemoveAt(coeffs.Last().Count - 1);
                }

                return new Tuple<double[,], double[,]>(coeffs.To2DArray(), results.To2DArray());
            }

        }
    }
}