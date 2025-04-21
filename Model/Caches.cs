using System;

namespace CSCache.Model
{
    public class Caches
    {
        public string IdCache { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; }
        public int? Detalle { get; set; }
        public string Informe { get; set; }
    }
}