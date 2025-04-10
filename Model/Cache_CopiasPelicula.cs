using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Cache_CopiasPelicula
    {
        public int CodPelicula { get; set; }
        public int CodCopia { get; set; }
        public int IdTecnologia { get; set; }
        public string Titulo { get; set; }
        public Nullable<int> CodIdioma { get; set; }
        public bool Subtitulada { get; set; }
        public bool Doblada { get; set; }
    }
}