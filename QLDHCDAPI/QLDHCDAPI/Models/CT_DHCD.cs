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
    
    public partial class CT_DHCD
    {
        public CT_DHCD()
        {
            this.CT_BAUBKS = new HashSet<CT_BAUBKS>();
            this.CT_BAUHDQT = new HashSet<CT_BAUHDQT>();
            this.UYQUYENs = new HashSet<UYQUYEN>();
            this.UYQUYENs1 = new HashSet<UYQUYEN>();
        }
    
        public string MATD { get; set; }
        public string MADH { get; set; }
        public int MACD { get; set; }
        public Nullable<long> SLCP { get; set; }
        public Nullable<long> SLDAUQ { get; set; }
        public Nullable<long> SLDCUQ { get; set; }
        public string HTDK { get; set; }
        public Nullable<long> SLCPSAUCUNG { get; set; }
        public Nullable<long> NCP_DABAUHDQT { get; set; }
        public Nullable<long> NCP_DABAUBKS { get; set; }
        public Nullable<long> NCP_DABQYKIEN { get; set; }
        public Nullable<long> NCP_DABQYKIEN_PHEU { get; set; }
    
        public virtual CODONG CODONG { get; set; }
        public virtual ICollection<CT_BAUBKS> CT_BAUBKS { get; set; }
        public virtual ICollection<CT_BAUHDQT> CT_BAUHDQT { get; set; }
        public virtual DHCD DHCD { get; set; }
        public virtual THANHVIENBK THANHVIENBK { get; set; }
        public virtual THANHVIENHDQT THANHVIENHDQT { get; set; }
        public virtual ICollection<UYQUYEN> UYQUYENs { get; set; }
        public virtual ICollection<UYQUYEN> UYQUYENs1 { get; set; }
    }
}
