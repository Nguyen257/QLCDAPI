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
using QLDHCDAPI.Controllers;

namespace QLDHCDAPI.API
{
    public class CoDongAPIController : ApiController
    {
        private QLDHCDEntities db = new QLDHCDEntities();

        [ResponseType(typeof(string))]
        public IHttpActionResult GetCoDongFromMaCD(string macd,string role)
        {

            UserCoDongController control = new UserCoDongController();
            CODONG cd = new CODONG();
            cd.HoTen = string.Empty;
            //string CurrentUserRole = control.GetRoleCurrentUser(username, pass);
            if (string.Equals(role, "Admin") || string.Equals(role, "User"))
            {
                if (!string.IsNullOrWhiteSpace(macd))
                {
                    try
                    {
                        int MaCoDong = int.Parse(macd);
                        List<CODONG> ListCD = (from l in db.CODONGs
                                               where l.MACD == MaCoDong
                                               select l).ToList();

                        if (ListCD.Count > 0)
                        {
                            cd = ListCD.First();
                            return Ok(cd.HoTen);
                        }
                        return Ok(cd.HoTen);
                    }
                    catch { return Ok(cd.HoTen); }

                }
                else
                    return Ok(cd.HoTen);
            }
            else
            {
                return Ok(cd.HoTen);
            }

            
        }

        //// GET api/CoDongAPI
        //public IQueryable<CODONG> GetCODONGs()
        //{
        //    return db.CODONGs;
        //}

        //// GET api/CoDongAPI/5
        //[ResponseType(typeof(CODONG))]
        //public IHttpActionResult GetCODONG(int id)
        //{
        //    CODONG codong = db.CODONGs.Find(id);
        //    if (codong == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(codong);
        //}

        //// PUT api/CoDongAPI/5
        //public IHttpActionResult PutCODONG(int id, CODONG codong)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != codong.MACD)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(codong).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CODONGExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// POST api/CoDongAPI
        //[ResponseType(typeof(CODONG))]
        //public IHttpActionResult PostCODONG(CODONG codong)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.CODONGs.Add(codong);
        //    db.SaveChanges();

        //    return CreatedAtRoute("DefaultApi", new { id = codong.MACD }, codong);
        //}

        //// DELETE api/CoDongAPI/5
        //[ResponseType(typeof(CODONG))]
        //public IHttpActionResult DeleteCODONG(int id)
        //{
        //    CODONG codong = db.CODONGs.Find(id);
        //    if (codong == null)
        //    {
        //        return NotFound();
        //    }

        //    db.CODONGs.Remove(codong);
        //    db.SaveChanges();

        //    return Ok(codong);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CODONGExists(int id)
        {
            return db.CODONGs.Count(e => e.MACD == id) > 0;
        }
    }
}