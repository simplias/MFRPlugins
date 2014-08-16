using System;

namespace DemoPlugin.ERPEntity
{
    public class Unternehmen
    {
        public string Name { get; set; }
        public string EindeutigeNummer { get; set; }
        public string Rechnung_Adresses { get; set; }
        public string Rechnung_Stadt { get; set; }
        public string Rechnung_Postleitzahl { get; set; }
        public DateTime Letzte_Aenderung { get; set; }
    }
}