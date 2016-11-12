﻿using System;
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
    public class DHCDAPIController : ApiController
    {
        private QLDHCDEntities db = new QLDHCDEntities();

        [ResponseType(typeof(int))]
        public IHttpActionResult GetMaCD(string thoiGian, string role)
        {
            int DataReturn = 0;
            if (string.Equals(role, "Admin") || string.Equals(role, "User"))
            {
                if (!string.IsNullOrWhiteSpace(thoiGian))
                {
                    try
                    {
                        DateTime DateThoiGian = DateTime.Now;
                        {
                            if (DateTime.TryParse(thoiGian, out DateThoiGian))
                            {
                                List<DHCD> ListDHCD = (from l in db.DHCDs
                                                       where l.thoiGian.Value.Year == DateThoiGian.Year
                                                       select l).ToList();
                                if (ListDHCD != null && ListDHCD.Count > 0)
                                {
                                    DataReturn = (ListDHCD.Max(x => x.STTDHTRONGNAM).HasValue) ? (ListDHCD.Max(x => x.STTDHTRONGNAM).Value) : 0;
                                }

                            }
                        }

                    }
                    catch { }

                }
            }
            else
            {
                
            }
            return Ok(DataReturn+1);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DHCDExists(string id)
        {
            return db.DHCDs.Count(e => e.MADH == id) > 0;
        }
    }
}