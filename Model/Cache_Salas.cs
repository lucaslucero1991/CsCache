using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Cache_Salas
    {
        public short CodSala { get; set; }
        public string NomSala { get; set; }
        public int CodTipoSala { get; set; }
        public string NomTipoSala { get; set; }
        public int CodComplejo { get; set; }
    }
}