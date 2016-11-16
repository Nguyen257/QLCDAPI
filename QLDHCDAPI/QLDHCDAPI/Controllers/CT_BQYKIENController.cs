using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLDHCDAPI.Models;
using PagedList;
using PagedList.Mvc;
using System.Net;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using QLDHCDAPI.Core;
using System.Globalization;
using System.Data.Entity.Core.Objects;


namespace QLDHCDAPI.Controllers
{
    public class CT_BQYKIENController : Controller
    {
        private QLDHCDEntities db = new QLDHCDEntities();

                DAO Dao = new DAO();
        CultureInfo culture = CultureInfo.CurrentCulture;

        public CT_BQYKIENController()
        {
            culture = new CultureInfo(1033);
        }

        // GET: /CT_BQYKIEN/
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            ViewBag.Alert = TempData["Message"] + string.Empty;
            ViewBag.ListAlert = TempData["listMess"];
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {

                List<CTBQYKIEN> lst = new List<CTBQYKIEN>();
                DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(q => q.thoiGian).First();

                lst = (from l in db.CTBQYKIENs where l.BANGYKIEN.DHCD.MADH == dhcd.MADH select l).ToList();
                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;
                if (!String.IsNullOrEmpty(searchString))
                {
                    lst = lst.Where(s => s.BANGYKIEN.NOIDUNG.Contains(searchString)).ToList();
                }
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(lst.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }

        // GET: /CT_BQYKIEN/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTBQYKIEN ctbqykien = db.CTBQYKIENs.Find(id);
            if (ctbqykien == null)
            {
                return HttpNotFound();
            }
            return View(ctbqykien);
        }

        // GET: /CT_BQYKIEN/Create
        public ActionResult Create()
        {
            ViewBag.MAYK = new SelectList(db.BANGYKIENs, "MAYK", "MADH");
            return View();
        }

        // POST: /CT_BQYKIEN/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,MAYK,MATD,LUACHON,NOIDUNGYKKHAC,THOIGIANBAU,LAHOPLE")] CTBQYKIEN ctbqykien)
        {
            if (ModelState.IsValid)
            {
                db.CTBQYKIENs.Add(ctbqykien);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MAYK = new SelectList(db.BANGYKIENs, "MAYK", "MADH", ctbqykien.MAYK);
            return View(ctbqykien);
        }

        // GET: /CT_BQYKIEN/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTBQYKIEN ctbqykien = db.CTBQYKIENs.Find(id);
            if (ctbqykien == null)
            {
                return HttpNotFound();
            }
            ViewBag.MAYK = new SelectList(db.BANGYKIENs, "MAYK", "MADH", ctbqykien.MAYK);
            return View(ctbqykien);
        }

        // POST: /CT_BQYKIEN/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,MAYK,MATD,LUACHON,NOIDUNGYKKHAC,THOIGIANBAU,LAHOPLE")] CTBQYKIEN ctbqykien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ctbqykien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MAYK = new SelectList(db.BANGYKIENs, "MAYK", "MADH", ctbqykien.MAYK);
            return View(ctbqykien);
        }

        // GET: /CT_BQYKIEN/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTBQYKIEN ctbqykien = db.CTBQYKIENs.Find(id);
            if (ctbqykien == null)
            {
                return HttpNotFound();
            }
            return View(ctbqykien);
        }

        // POST: /CT_BQYKIEN/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CTBQYKIEN ctbqykien = db.CTBQYKIENs.Find(id);
            db.CTBQYKIENs.Remove(ctbqykien);
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
