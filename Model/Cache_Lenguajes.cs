using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Cache_Lenguajes
    {
        public Cache_Lenguajes()
        {
            this.Cache_Peliculas = new List<Cache_Peliculas>();
        }

        public int CodLenguaje { get; set; }
        public string NomLenguaje { get; set; }
        public virtual List<Cache_Peliculas> Cache_Peliculas { get; set; }
    }
}