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
    public class dsdaController : Controller
    {
        private QLDHCDEntities db = new QLDHCDEntities();

        // GET: /dsda/
        public ActionResult Index()
        {
            return View(db.DHCDs.ToList());
        }

        // GET: /dsda/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DHCD dhcd = db.DHCDs.Find(id);
            if (dhcd == null)
            {
                return HttpNotFound();
            }
            return View(dhcd);
        }

        // GET: /dsda/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /dsda/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="MADH,TenDH,YEARDHCD,STTDHTRONGNAM,nQKin,nQBefore,nDeCuHDQT,nUngCuHDQT,nDeCuBKS,nUngCuBKS,nBauBoSungHDQT,nBauBOSungBKS,thoiGian,ACTIVE,TONGSOPHIEU,LABAUBOSUNG,TRANGTHAI")] DHCD dhcd)
        {
            if (ModelState.IsValid)
            {
                db.DHCDs.Add(dhcd);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dhcd);
        }

        // GET: /dsda/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DHCD dhcd = db.DHCDs.Find(id);
            if (dhcd == null)
            {
                return HttpNotFound();
            }
            return View(dhcd);
        }

        // POST: /dsda/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="MADH,TenDH,YEARDHCD,STTDHTRONGNAM,nQKin,nQBefore,nDeCuHDQT,nUngCuHDQT,nDeCuBKS,nUngCuBKS,nBauBoSungHDQT,nBauBOSungBKS,thoiGian,ACTIVE,TONGSOPHIEU,LABAUBOSUNG,TRANGTHAI")] DHCD dhcd)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dhcd).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dhcd);
        }

        // GET: /dsda/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DHCD dhcd = db.DHCDs.Find(id);
            if (dhcd == null)
            {
                return HttpNotFound();
            }
            return View(dhcd);
        }

        // POST: /dsda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DHCD dhcd = db.DHCDs.Find(id);
            db.DHCDs.Remove(dhcd);
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
