using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using QLDHCDAPI.Models;

namespace QLDHCDAPI.API
{
    public class THANHVIENBKSAPIController : ApiController
    {
        private QLDHCDEntities db = new QLDHCDEntities();

        // GET api/THANHVIENHDQTAPI
        public IHttpActionResult GetTHANHVIENHDQTs(string role)
        {
            if (role == "User" || role == "Admin")
            {
                try
                {
                    DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(q => q.thoiGian).ToList().First();
                    var v = (from l in db.THANHVIENBKS
                             where l.CT_DHCD.MADH == dhcd.MADH
                             select new
                             {
                                 MATD = l.MATD,
                                 HoTen = l.CT_DHCD.CODONG.HoTen
                             });
                    var Result = new
                    {
                        success = "success",
                        data = v
                    };
                    return Json(Result);
                }
                catch
                {
                    var Result = new
                    {
                        success = "NoSuccess",
                        data = new List<string>()
                    };
                    return Json(Result);
                }
            }
            else
            {
                var Result = new
                {
                    success = "NoSuccess",
                    data = new List<string>()
                };
                return Json(Result);
            }


        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool THANHVIENBKExists(string id)
        {
            return db.THANHVIENBKS.Count(e => e.MATD == id) > 0;
        }
    }
}