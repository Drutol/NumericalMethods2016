using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace NumMethods1.Utils
{
    public enum AvailableLocale
    {
        PL,
        EN
    }

    public static class LocalizationManager
    {
        static LocalizationManager()
        {
            using (var writer = new StreamReader(@"./Localization/pl_PL.json"))
            {
                PlDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(writer.ReadToEnd());
            }
            using (var writer = new StreamReader(@"./Localization/en_GB.json"))
            {
                EnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(writer.ReadToEnd());
            }
        }

        public static Dictionary<string, string> EnDictionary { get; }
        public static Dictionary<string, string> PlDictionary { get; }

        public static AvailableLocale GetNextLocale(AvailableLocale locale)
        {
            return (int) locale + 1 > 1 ? AvailableLocale.PL : locale + 1;
        }
    }
}