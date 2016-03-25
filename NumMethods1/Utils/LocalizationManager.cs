using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace NumMethods1.Utils
{
    public enum AvailableLocale
    {
        PL,
        EN,
    }

    public static class LocalizationManager
    {
        public static Dictionary<string,string> EnDictionary { get; }
        public static Dictionary<string,string> PlDictionary { get; } 

        static LocalizationManager ()
        {
            using (StreamReader writer = new StreamReader(@"./Localization/pl_PL.json"))
            {
                PlDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(writer.ReadToEnd());
            }
            using (StreamReader writer = new StreamReader(@"./Localization/en_GB.json"))
            {
                EnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(writer.ReadToEnd());
            }
        }
    }
}
