using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Complejos
    {
        public Complejos()
        {
            this.Cache_Funciones = new List<Cache_Funciones>();
            this.Cache_GruposSemana = new List<Cache_GruposSemana>();
            this.Cache_Productos = new List<Cache_Productos>();
            this.Cache_Salas = new List<Cache_Salas>();
            //this.Cache_KeyLayouts = new List<Cache_KeyLayouts>();
            //this.ImagenesComplejos = new List<ImagenesComplejos>();
            //this.Prom_Promotions = new List<Prom_Promotions>();
        }

        public int CodComplejo { get; set; }
        public string NomComplejo { get; set; }

        public virtual Cache_Cinesemanas Cache_Cinesemanas { get; set; }
        public virtual List<Cache_Funciones> Cache_Funciones { get; set; }
        public virtual List<Cache_GruposSemana> Cache_GruposSemana { get; set; }
        public List<Cache_Productos> Cache_Productos { get; set; }
        public virtual List<Cache_Salas> Cache_Salas { get; set; }
        //public virtual List<Cache_KeyLayouts> Cache_KeyLayouts { get; set; }
        //public virtual List<ImagenesComplejos> ImagenesComplejos { get; set; }
        //public virtual List<Prom_Promotions> Prom_Promotions { get; set; }
    }
}