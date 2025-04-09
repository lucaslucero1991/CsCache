using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Cache_Peliculas
    {
        /*
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cache_Peliculas()
        {
            this.Cache_Actores = new HashSet<Cache_Actores>();
            this.Cache_Directores = new HashSet<Cache_Directores>();
        }
        */

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

        /*
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cache_Actores> Cache_Actores { get; set; }
        public virtual Cache_Clasificaciones Cache_Clasificaciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cache_Directores> Cache_Directores { get; set; }
        public virtual Cache_Generos Cache_Generos { get; set; }
        public virtual Cache_Lenguajes Cache_Lenguajes { get; set; }
        */
    }
}