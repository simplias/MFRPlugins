using System;

namespace DemoPlugin.ERPEntity
{
    public class Auftrag
    {
        public string Name { get; set; }
        public string EindeutigeNummer { get; set; }

        public string TechnikerId { get; set; }

        public DateTime TerminBegin { get; set; }
        public DateTime TerminEnde { get; set; }
    }
}