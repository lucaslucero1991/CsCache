using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Cache_Funciones
    {
        public int CodFuncion { get; set; }
        public int CodComplejo { get; set; }
        public string HoraComienzo { get; set; }
        public short Vuelta { get; set; }
        public string Estado { get; set; }
        public bool Preestreno { get; set; }
        public int ButacasDisponibles { get; set; }
        public int ButacasHabilitadas { get; set; }
        public int CodDistribucion { get; set; }
        public System.DateTime Fecha { get; set; }
        public short CodSala { get; set; }
        public int CodTecnologia { get; set; }
        public int CodPelicula { get; set; }
        public Nullable<int> CodCopia { get; set; }
        public Complex_Options Complex_Options { get; set; }
        public Cache_Tecnologias Cache_Tecnologias { get; set; }
        //public Complejos Complejos { get; set; }
    }
}