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

    }
}