using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLDHCDAPI.Models;

namespace QLDHCDAPI.Controllers
{
    public class AASSController : Controller
    {
        private QLDHCDEntities db = new QLDHCDEntities();

        // GET: /AASS/
        public ActionResult Index()
        {
            var usercds = db.USERCDs.Include(u => u.CODONG);
            return View(usercds.ToList());
        }

        // GET: /AASS/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USERCD usercd = db.USERCDs.Find(id);
            if (usercd == null)
            {
                return HttpNotFound();
            }
            return View(usercd);
        }

        // GET: /AASS/Create
        public ActionResult Create()
        {
            ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen");
            return View();
        }

        // POST: /AASS/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,USERNAME,PASS,MACD")] USERCD usercd)
        {
            if (ModelState.IsValid)
            {
                db.USERCDs.Add(usercd);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen", usercd.MACD);
            return View(usercd);
        }

        // GET: /AASS/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USERCD usercd = db.USERCDs.Find(id);
            if (usercd == null)
            {
                return HttpNotFound();
            }
            ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen", usercd.MACD);
            return View(usercd);
        }

        // POST: /AASS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,USERNAME,PASS,MACD")] USERCD usercd)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usercd).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen", usercd.MACD);
            return View(usercd);
        }

        // GET: /AASS/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USERCD usercd = db.USERCDs.Find(id);
            if (usercd == null)
            {
                return HttpNotFound();
            }
            return View(usercd);
        }

        // POST: /AASS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            USERCD usercd = db.USERCDs.Find(id);
            db.USERCDs.Remove(usercd);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
