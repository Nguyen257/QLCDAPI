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
    
    public partial class CTBQYKIEN
    {
        public int ID { get; set; }
        public int MAYK { get; set; }
        public string MATD { get; set; }
        public Nullable<int> LUACHON { get; set; }
        public string NOIDUNGYKKHAC { get; set; }
        public Nullable<System.DateTime> THOIGIANBAU { get; set; }
        public bool LAHOPLE { get; set; }
    
        public virtual BANGYKIEN BANGYKIEN { get; set; }
        public virtual CT_DHCD CT_DHCD { get; set; }
    }
}
