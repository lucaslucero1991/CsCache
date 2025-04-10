using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Cache_Clasificaciones
    {
        
        public Cache_Clasificaciones()
        {
            this.Cache_Peliculas = new List<Cache_Peliculas>();
        }

        public short CodClasificacion { get; set; }
        public string NomClasificacion { get; set; }
      
        public List<Cache_Peliculas> Cache_Peliculas { get; set; }
    }
}