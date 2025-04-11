using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Cache_Cinesemanas
    {
        public int CodComplejo { get; set; }
        public System.DateTime Desde { get; set; }
        public System.DateTime Hasta { get; set; }
        public List<Cache_GruposSemana> Cache_GruposSemana { get; set; }
    }
}