using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class Complex_Options
    {
        public int CodComplejo { get; set; }
        public int CodTerminal { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string PassEmail { get; set; }
        public string SMTPServer { get; set; }
        public bool? EnableSSL { get; set; }
        public string PortNumber { get; set; }
        public string GoogleAnalytics { get; set; }
        public bool Allow_Sale { get; set; }
        public bool Allow_ConcessionSale { get; set; }
        public Nullable<bool> Request_TaxId { get; set; }
        public Nullable<int> Tolerancia { get; set; }
        public Nullable<int> MaxSales { get; set; }
        public string Hourly { get; set; }
        public string ComplexDetails { get; set; }
        public Nullable<int> Request_MinRangeTax { get; set; }
        public Nullable<int> Request_MaxRangeTax { get; set; }
        public string Barcode { get; set; }
        public Nullable<int> PickupCode { get; set; }
        public string AuxEmail { get; set; }
        public string AuxSMTPServer { get; set; }
        public Nullable<int> AuxPort { get; set; }
        public string AuxPassword { get; set; }
        public Nullable<bool> AuxEnableSSL { get; set; }
        public Nullable<int> MinimunSeatToSell { get; set; }
        public Nullable<bool> Allow_Refund { get; set; }
        public string Direccion { get; set; }
        public List<Cache_Salas> Cache_Salas { get; set; }
    }
}