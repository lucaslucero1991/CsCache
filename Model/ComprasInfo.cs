//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CSCache.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class ComprasInfo
    {
        public long id { get; set; }
        public long IdVenta { get; set; }
        public System.DateTime FechaRealFuncion { get; set; }
        public int CodPelicula { get; set; }
        public string TituloPelicula { get; set; }
        public decimal CodCliente { get; set; }
        public int CodComplejo { get; set; }
        public string CodigoRetiro { get; set; }
        public string Entradas { get; set; }
        public string Fecha { get; set; }
        public Nullable<byte> Cancelada { get; set; }
        public Nullable<System.DateTime> FechaCancelada { get; set; }
    
        public virtual Ventas Ventas { get; set; }
    }
}
