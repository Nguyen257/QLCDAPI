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
    public class THAMDUDHAPIController : ApiController
    {
        private QLDHCDEntities db = new QLDHCDEntities();

       

        // GET api/THAMDUDHAPI/5
        public IHttpActionResult GetCoDongFromMaTD(string id,string role)
        {
           
            try
            {
                if (role == "User" || role == "Admin")
                {
                    var listThamDu = (from l in db.CT_DHCD
                                      where l.MATD == id
                                      select new
                                      {
                                          MATD = l.MATD,
                                          HoTen = l.CODONG.HoTen,
                                          CMND = (l.CODONG.CMND.HasValue) ? (l.CODONG.CMND.Value) : 0,
                                          NgayCap = l.CODONG.NgayCap,
                                          NoiCap = l.CODONG.NoiCap,
                                          DiaChi = l.CODONG.DiaChi,
                                          QuocTich = l.CODONG.QuocTich
                                      });
                    if (listThamDu == null || listThamDu.Count() == 0)
                    {
                        var Result = new
                        {
                            success = "NoSuccess",
                            data = new List<string>()
                        };
                        return Json(Result);
                    }

                    var dataResult = new
                    {
                        success = "success",
                        data = listThamDu.First()
                    };

                    return Json(dataResult);
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
            catch(Exception ex)
            {
                var Result = new
                {
                    success = "NoSuccess",
                    exception = ex.Message + string.Empty,
                    data = new List<string>()
                };
                return Json(Result);
            }
            
        }

        [ResponseType(typeof(string))]
        public IHttpActionResult GetCoDongFromMaCD(string id, string role)
        {

            try
            {
                if (role == "User" || role == "Admin")
                {
                    DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(x => x.thoiGian).First();
                    var listThamDu = (from l in db.CT_DHCD
                                      where l.CODONG.MACD == int.Parse(id) && l.MADH == dhcd.MADH
                                      select l.CODONG.HoTen);
                    if (listThamDu == null || listThamDu.Count() == 0)
                    {
                        return Ok("");
                    }

                    return Ok(listThamDu.First());
                }
                else
                {
                    return Ok("");
                }
            }
            catch (Exception ex)
            {
                return Ok("");
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

        private bool CT_DHCDExists(string id)
        {
            return db.CT_DHCD.Count(e => e.MATD == id) > 0;
        }
    }
}