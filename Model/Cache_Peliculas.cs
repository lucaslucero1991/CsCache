using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Cache_Peliculas
    {
        
        public Cache_Peliculas()
        {
            this.Actores = new List<string>();
            this.Directores = new List<string>();
        }
       
        public int CodPelicula { get; set; }
        public string Titulo { get; set; }
        public string TituloOriginal { get; set; }
        public bool Subtitulada { get; set; }
        public int Duracion { get; set; }
        public System.DateTime Estreno { get; set; }
        public short CodClasificacion { get; set; }
        public string Sinopsis { get; set; }
        public string SinopsisCorta { get; set; }
        public string Web1 { get; set; }
        public string Web2 { get; set; }
        public string UrlTrailer { get; set; }
        public int CodGenero { get; set; }
        public int CodLenguaje { get; set; }
        public string Filename { get; set; }
        public Cache_Clasificaciones Cache_Clasificaciones { get; set; }
        public Cache_Generos Cache_Generos { get; set; }
        public Cache_Lenguajes Cache_Lenguajes { get; set; }
        public List<string> Actores { get; set; }
        public List<string> Directores { get; set; }

        
    }
}