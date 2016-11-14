using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLDHCDAPI.Models
{
    public class DHCD_ThanhVien
    {
        public DHCD DHCD { get; set; }

        public THANHVIENHDQT lstHDQT {get;set;}

        public THANHVIENBK lstBKS { get; set; }
        public CT_DHCD THAMDU {get;set;}
    }
}