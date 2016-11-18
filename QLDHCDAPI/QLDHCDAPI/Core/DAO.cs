using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QLDHCDAPI.Models;

namespace QLDHCDAPI.Core
{
    public class DAO
    {
        QLDHCDEntities db = new QLDHCDEntities();
        
        internal bool SaveException(string methodName, string exceptionMessage)
        {
            bool DataReturn = true;
            try
            {
                string UserName = HttpContext.Current.Session["UserName"] + string.Empty;
                EXCEPTIONAPP ex = new EXCEPTIONAPP();
                ex.TITLE = "Lỗi " + methodName + " - " + exceptionMessage;
                ex.ERRORUSER = UserName;

                ex.ERRORDATE = DateTime.Now;

                db.EXCEPTIONAPPs.Add(ex);
                db.SaveChanges();

            }
            catch
            {
                return false;
            }
            return DataReturn;
        }

        internal bool ChotCoDongThamDuFunc(string Role)
        {
            if (Role == "Admin")
            {
                DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(q => q.thoiGian).First();
                long tongSoCPCoDong = db.CT_DHCD.Where(x => x.MADH == dhcd.MADH).Sum(q => q.SLCP) ?? 0;
                int TichSoHDQT = 0;
                int TichSoBKS = 0;
                if (dhcd.LABAUBOSUNG)
                {
                    TichSoHDQT = dhcd.nBauBoSungHDQT ?? 0;
                    TichSoBKS = dhcd.nBauBOSungBKS ?? 0;
                }
                else
                {
                    TichSoHDQT = dhcd.nDeCuHDQT ?? 0 + dhcd.nUngCuHDQT ?? 0;
                    TichSoBKS = dhcd.nDeCuBKS ?? 0 + dhcd.nUngCuBKS ?? 0;
                }
                int TongSOPhieu = db.CT_DHCD.Where(x => x.MADH == dhcd.MADH).Count();
                dhcd.SLCPPHATRA_HDQT = tongSoCPCoDong * TichSoHDQT;
                dhcd.SLCPPHATRA_BKS = tongSoCPCoDong * TichSoBKS;
                dhcd.TONGSOPHIEU = TongSOPhieu;

                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckCoDongThamDuDH(int macd, string madh)
        {
            bool DataReturn = false;
            try
            {
                List<CT_DHCD> listTD = db.CT_DHCD.Where(x => x.MACD == macd && x.MADH == madh).ToList();
                if (listTD != null && listTD.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return DataReturn;
        }
    }
}