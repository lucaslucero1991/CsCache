using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Cache_Productos
    {
        public int CodProducto { get; set; }
        public int CodComplejo { get; set; }
        public string NomProducto { get; set; }
        public Nullable<decimal> Precio { get; set; }
        public short Posicion { get; set; }
        public string NombreArchivo { get; set; }
        public Complejos Complejos { get; set; }
    }
}