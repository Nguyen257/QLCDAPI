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
    public class THANHVIENBKSController : Controller
    {
        private QLDHCDEntities db = new QLDHCDEntities();

        // GET: /THANHVIENBKS/
        public ActionResult Index()
        {
            var thanhvienbks = db.THANHVIENBKS.Include(t => t.CODONG).Include(t => t.DHCD);
            return View(thanhvienbks.ToList());
        }

        // GET: /THANHVIENBKS/Details/5
        public ActionResult Details(int macd,string madh)
        {
            if (macd==null || string.IsNullOrEmpty(madh))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<THANHVIENBK> ListThanhVienBKS = (from l in db.THANHVIENBKS
                                        where l.MACD == macd && l.MADH == madh
                                        select l).ToList();
            THANHVIENBK ThanhVienbks = (ListThanhVienBKS.Count > 0) ? (ListThanhVienBKS.First()) : (null) ;
            //THANHVIENBK thanhvienbk = db.THANHVIENBKS.Find(macd, madh);
            if (ThanhVienbks == null)
            {
                return HttpNotFound();
            }
            return View(ThanhVienbks);
        }

        // GET: /THANHVIENBKS/Create
        public ActionResult Create()
        {
            ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen");
            ViewBag.MADH = new SelectList(db.DHCDs, "MADH", "TenDH");
            return View();
        }

        // POST: /THANHVIENBKS/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="MADH,MACD,SOPHIEUBAU,LATHAYTHE")] THANHVIENBK thanhvienbk)
        {
            if (ModelState.IsValid)
            {
                db.THANHVIENBKS.Add(thanhvienbk);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen", thanhvienbk.MACD);
            ViewBag.MADH = new SelectList(db.DHCDs, "MADH", "TenDH", thanhvienbk.MADH);
            return View(thanhvienbk);
        }

        // GET: /THANHVIENBKS/Edit/5
        public ActionResult Edit(int macd, string madh)
        {
            if (macd == null || string.IsNullOrEmpty(madh))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<THANHVIENBK> ListThanhVienBKS = (from l in db.THANHVIENBKS
                                        where l.MACD == macd && l.MADH == madh
                                        select l).ToList();
            THANHVIENBK thanhvienbk = (ListThanhVienBKS.Count > 0) ? (ListThanhVienBKS.First()) : (null);
            //THANHVIENBK thanhvienbk = db.THANHVIENBKS.Find(id);
            if (thanhvienbk == null)
            {
                return HttpNotFound();
            }
            ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen", thanhvienbk.MACD);
            ViewBag.MADH = new SelectList(db.DHCDs, "MADH", "TenDH", thanhvienbk.MADH);
            return View(thanhvienbk);
        }

        // POST: /THANHVIENBKS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="MADH,MACD,SOPHIEUBAU,LATHAYTHE")] THANHVIENBK thanhvienbk)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thanhvienbk).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen", thanhvienbk.MACD);
            ViewBag.MADH = new SelectList(db.DHCDs, "MADH", "TenDH", thanhvienbk.MADH);
            return View(thanhvienbk);
        }

        // GET: /THANHVIENBKS/Delete/5
        public ActionResult Delete(int macd, string madh)
        {
            if (macd == null || string.IsNullOrEmpty(madh))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<THANHVIENBK> ListThanhVienBKS = (from l in db.THANHVIENBKS
                                        where l.MACD == macd && l.MADH == madh
                                        select l).ToList();
            THANHVIENBK thanhvienbk = (ListThanhVienBKS.Count > 0) ? (ListThanhVienBKS.First()) : (null);
            //THANHVIENBK thanhvienbk = db.THANHVIENBKS.Find(id);
            if (thanhvienbk == null)
            {
                return HttpNotFound();
            }
            return View(thanhvienbk);
        }

        // POST: /THANHVIENBKS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int macd, string madh)
        {
            List<THANHVIENBK> ListThanhVienBKS = (from l in db.THANHVIENBKS
                                        where l.MACD == macd && l.MADH == madh
                                        select l).ToList();
            THANHVIENBK thanhvienbk = (ListThanhVienBKS.Count > 0) ? (ListThanhVienBKS.First()) : (null);
            //THANHVIENBK thanhvienbk = db.THANHVIENBKS.Find(id);
            db.THANHVIENBKS.Remove(thanhvienbk);
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
