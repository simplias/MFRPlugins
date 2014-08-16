using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace DemoPlugin.ERPEntity
{
    public class Benutzer
    {
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string EindeutigeNummer { get; set; }
    }
}