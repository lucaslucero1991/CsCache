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
    
    public partial class banners
    {
        public long id { get; set; }
        public byte[] image { get; set; }
        public Nullable<int> banner_order { get; set; }
        public Nullable<byte> online { get; set; }
        public string target { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public string imagebase64 { get; set; }
        public string filename { get; set; }
    }
}
