using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace DemoPlugin.ERPEntity
{
    public class Einsatzort
    {
        public string Name { get; set; }
        public string EindeutigeNummer { get; set; }
        public string Adresse { get; set; }
        public string Postleitzahl { get; set; }
        public string Stadt { get; set; }
        public string Beschreibung { get; set; }
        public DateTime LetzteAenderung { get; set; }
    }
}