//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QLDHCDAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UYQUYEN
    {
        public int MAUQ { get; set; }
        public string MADH { get; set; }
        public string MANGCHUYEN { get; set; }
        public string UYQUYENTYPE { get; set; }
        public string MANGNHAN { get; set; }
        public Nullable<long> SLUQ { get; set; }
        public Nullable<System.DateTime> THOIGIAN { get; set; }
    
        public virtual CT_DHCD CT_DHCD { get; set; }
        public virtual CT_DHCD CT_DHCD1 { get; set; }
        public virtual DHCD DHCD { get; set; }
    }
}
