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
    
    public partial class BANGYKIENBQPHIEU
    {
        public BANGYKIENBQPHIEU()
        {
            this.CT_YKIEN_BQPHIEU = new HashSet<CT_YKIEN_BQPHIEU>();
        }
    
        public int MAYK { get; set; }
        public string MADH { get; set; }
        public string NOIDUNG { get; set; }
        public int SLDONGY { get; set; }
        public long NCPDONGY { get; set; }
        public int SLKHONGDONGY { get; set; }
        public long NCPKHONGDONGY { get; set; }
        public int SL_PHATRA { get; set; }
        public long SLCP_PHATRA { get; set; }
        public int SL_THUVAO { get; set; }
        public long SLCP_THUVAO { get; set; }
        public int SL_HOPLE { get; set; }
        public long SLCP_HOPLE { get; set; }
        public int SL_KHONG_HOPLE { get; set; }
        public long SLCP_KHONG_HOPLE { get; set; }
    
        public virtual ICollection<CT_YKIEN_BQPHIEU> CT_YKIEN_BQPHIEU { get; set; }
        public virtual DHCD DHCD { get; set; }
    }
}
