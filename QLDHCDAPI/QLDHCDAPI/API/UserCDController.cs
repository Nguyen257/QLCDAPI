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
using System.Security.Cryptography;
using System.Text;

namespace QLDHCDAPI.API
{
    public class UserCDController : ApiController
    {
        private QLDHCDEntities db = new QLDHCDEntities();

        // GET api/UserCD
        public IQueryable<USERCD> GetUSERCDs()
        {
            return db.USERCDs;
        }

        // GET api/UserCD/5
        [ResponseType(typeof(USERCD))]
        public IHttpActionResult GetUSERCD(int id)
        {
            USERCD usercd = db.USERCDs.Find(id);
            if (usercd == null)
            {
                return NotFound();
            }

            return Ok(usercd);
        }

        [ResponseType(typeof(USERCD))]
        public IHttpActionResult LoginUser(string userName,string pass)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes;
            UTF8Encoding encoder = new UTF8Encoding();
            hashedDataBytes =
              md5Hasher.ComputeHash(encoder.GetBytes(pass));

            USERCD usercd = (from l in db.USERCDs
                               where l.USERNAME == userName && l.PASS == hashedDataBytes
                               select l).First();

            //USERCD usercd = db.USERCDs.Find(id);
            if (usercd == null)
            {
                return NotFound();
            }

            return Ok(usercd);
        }


        // PUT api/UserCD/5
        public IHttpActionResult PutUSERCD(int id, USERCD usercd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != usercd.ID)
            {
                return BadRequest();
            }

            db.Entry(usercd).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!USERCDExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/UserCD
        [ResponseType(typeof(USERCD))]
        public IHttpActionResult PostUSERCD(USERCD usercd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.USERCDs.Add(usercd);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = usercd.ID }, usercd);
        }

        public IHttpActionResult CreateUser(string userName,string pass)
        {
            string HashMd5 = string.Empty;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedBytes;
            UTF8Encoding encoder = new UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(pass));

            USERCD usercd = new USERCD();
            usercd.USERNAME = userName;
            usercd.PASS = hashedBytes;
                     

            db.USERCDs.Add(usercd);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = usercd.ID }, usercd);
        }

        // DELETE api/UserCD/5
        [ResponseType(typeof(USERCD))]
        public IHttpActionResult DeleteUSERCD(int id)
        {
            USERCD usercd = db.USERCDs.Find(id);
            if (usercd == null)
            {
                return NotFound();
            }

            db.USERCDs.Remove(usercd);
            db.SaveChanges();

            return Ok(usercd);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool USERCDExists(int id)
        {
            return db.USERCDs.Count(e => e.ID == id) > 0;
        }
    }
}