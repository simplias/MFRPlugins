using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace DemoPlugin.ERPEntity
{
    public class Kontakt
    {
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string EindeutigeNummer { get; set; }
        public string Berufsbezeichung { get; set; }
        public string EMail { get; set; }
        public string Fax { get; set; }
        public string Handy { get; set; }
        public string Telephone { get; set; }
        public DateTime LetzteAenderung { get; set; }
    }
}