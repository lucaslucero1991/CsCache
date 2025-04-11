using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Cache_Generos
    {
        
        public Cache_Generos()
        {
            this.Cache_Peliculas = new List<Cache_Peliculas>();
        }

        public int CodGenero { get; set; }
        public string NomGenero { get; set; }
        public  List<Cache_Peliculas> Cache_Peliculas { get; set; }
    }
}
