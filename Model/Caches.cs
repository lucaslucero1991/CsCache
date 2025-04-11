using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Caches
    {
        public string IdCache { get; set; }
        //public Nullable<System.DateTime> UltimaActualizacion { get; set; }
        public Nullable<System.DateTime> FechaInicio { get; set; }
        public Nullable<System.DateTime> FechaFin { get; set; }
        public string Estado { get; set; }
        public Nullable<int> Detalle { get; set; }
        public string Informe { get; set; }
    }
}