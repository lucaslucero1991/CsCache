using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Feature
    {
        public int FeatureId { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public bool SubTitled { get; set; }
        public bool Dubbed { get; set; }
        public int TotalRuntime { get; set; }
        public string Rating { get; set; }
        public string Genre { get; set; }
        public DateTime PremierDate { get; set; }
        public string Language { get; set; }
        public string ShortSynopsis { get; set; }
        public int TechnologyID { get; set; }
        public string TechnologyName { get; set; }
    }
}