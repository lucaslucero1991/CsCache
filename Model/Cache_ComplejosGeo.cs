using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Cache_ComplejosGeo
    {
        public int CodComplejo { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public int Zoom { get; set; }
        public string CodigoPostal { get; set; }
        public string Extension { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }
}