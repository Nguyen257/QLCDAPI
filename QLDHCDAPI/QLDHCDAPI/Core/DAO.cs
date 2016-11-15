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