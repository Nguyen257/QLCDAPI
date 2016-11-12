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
    public class ThanhVienHDQTController : Controller
    {
        private QLDHCDEntities db = new QLDHCDEntities();

        // GET: /ThanhVienHDQT/
        public ActionResult Index()
        {
            var thanhvienhdqts = db.THANHVIENHDQTs.Include(t => t.CODONG).Include(t => t.DHCD);
            return View(thanhvienhdqts.ToList());
        }

        // GET: /ThanhVienHDQT/Details/5
        public ActionResult Details(int macd, string madh)
        {
            if (macd == null || string.IsNullOrEmpty(madh))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<THANHVIENHDQT> ListThanhVienHDQT = (from l in db.THANHVIENHDQTs
                                                     where l.MACD == macd && l.MADH == madh
                                                     select l).ToList();
            THANHVIENHDQT thanhvienhdqt = (ListThanhVienHDQT.Count>0)?(ListThanhVienHDQT.First()):(null);
            //THANHVIENHDQT thanhvienhdqt = db.THANHVIENHDQTs.Find(id);
            if (thanhvienhdqt == null)
            {
                return HttpNotFound();
            }
            return View(thanhvienhdqt);
        }

        // GET: /ThanhVienHDQT/Create
        public ActionResult Create()
        {
            ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen");
            ViewBag.MADH = new SelectList(db.DHCDs, "MADH", "TenDH");
            return View();
        }

        // POST: /ThanhVienHDQT/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="MADH,MACD,SOPHIEUBAU,LATHAYTHE")] THANHVIENHDQT thanhvienhdqt)
        {
            if (ModelState.IsValid)
            {
                db.THANHVIENHDQTs.Add(thanhvienhdqt);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen", thanhvienhdqt.MACD);
            ViewBag.MADH = new SelectList(db.DHCDs, "MADH", "TenDH", thanhvienhdqt.MADH);
            return View(thanhvienhdqt);
        }

        // GET: /ThanhVienHDQT/Edit/5
        public ActionResult Edit(int macd, string madh)
        {
            if (macd == null || string.IsNullOrEmpty(madh))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<THANHVIENHDQT> ListThanhVienHDQT = (from l in db.THANHVIENHDQTs
                                                     where l.MACD == macd && l.MADH == madh
                                                     select l).ToList();
            THANHVIENHDQT thanhvienhdqt = (ListThanhVienHDQT.Count > 0) ? (ListThanhVienHDQT.First()) : (null);
            //THANHVIENHDQT thanhvienhdqt = db.THANHVIENHDQTs.Find(id);
            if (thanhvienhdqt == null)
            {
                return HttpNotFound();
            }
            ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen", thanhvienhdqt.MACD);
            ViewBag.MADH = new SelectList(db.DHCDs, "MADH", "TenDH", thanhvienhdqt.MADH);
            return View(thanhvienhdqt);
        }

        // POST: /ThanhVienHDQT/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="MADH,MACD,SOPHIEUBAU,LATHAYTHE")] THANHVIENHDQT thanhvienhdqt)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thanhvienhdqt).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen", thanhvienhdqt.MACD);
            ViewBag.MADH = new SelectList(db.DHCDs, "MADH", "TenDH", thanhvienhdqt.MADH);
            return View(thanhvienhdqt);
        }

        // GET: /ThanhVienHDQT/Delete/5
        public ActionResult Delete(int macd, string madh)
        {
            if (macd == null || string.IsNullOrEmpty(madh))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<THANHVIENHDQT> ListThanhVienHDQT = (from l in db.THANHVIENHDQTs
                                                     where l.MACD == macd && l.MADH == madh
                                                     select l).ToList();
            THANHVIENHDQT thanhvienhdqt = (ListThanhVienHDQT.Count > 0) ? (ListThanhVienHDQT.First()) : (null);
            //THANHVIENHDQT thanhvienhdqt = db.THANHVIENHDQTs.Find(id);
            if (thanhvienhdqt == null)
            {
                return HttpNotFound();
            }
            return View(thanhvienhdqt);
        }

        // POST: /ThanhVienHDQT/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int macd, string madh)
        {
            if(macd==null || string.IsNullOrEmpty(madh))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<THANHVIENHDQT> ListThanhVienHDQT = (from l in db.THANHVIENHDQTs
                                                     where l.MACD == macd && l.MADH == madh
                                                     select l).ToList();
            THANHVIENHDQT thanhvienhdqt = (ListThanhVienHDQT.Count > 0) ? (ListThanhVienHDQT.First()) : (null);
            //THANHVIENHDQT thanhvienhdqt = db.THANHVIENHDQTs.Find(id);
            db.THANHVIENHDQTs.Remove(thanhvienhdqt);
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
