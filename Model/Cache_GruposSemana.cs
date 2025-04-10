using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Cache_GruposSemana
    {
        public string NomGrupo { get; set; }
        public int Orden { get; set; }
        public System.DateTime Desde { get; set; }
        public System.DateTime Hasta { get; set; }
        public int CodComplejo { get; set; }

        //public Complejos Complejos { get; set; }
    }
}