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
    
    public partial class LogAgent
    {
        public System.Guid Id { get; set; }
        public string SessionKey { get; set; }
        public string BuySingleConfirmResponse { get; set; }
        public Nullable<System.DateTime> BuySingleConfirmResponseTime { get; set; }
        public string ShopProcessId { get; set; }
        public string Amount { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string ExtendedResponseDescription { get; set; }
        public string ResponseDetails { get; set; }
        public Nullable<bool> IsFinishedPageFired { get; set; }
        public string FinishedRedirectUrl { get; set; }
        public string ClaveCinestar { get; set; }
        public Nullable<bool> IsRollback { get; set; }
        public Nullable<System.DateTime> RollbackDatetime { get; set; }
        public string RollbackResponseData { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public string Complex { get; set; }
        public int State { get; set; }
        public int Retries { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public Nullable<int> AgentId { get; set; }
        public string OrderId { get; set; }
        public string LanguageId { get; set; }
        public string SaleInformation { get; set; }
    }
}
