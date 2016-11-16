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


namespace QLDHCDAPI.Controllers
{
    public class BIEUQUYETYKIENController : Controller
    {
        private QLDHCDEntities db = new QLDHCDEntities();
        DAO Dao = new DAO();
        CultureInfo culture = CultureInfo.CurrentCulture;

        public bool checkThanhVien(string matd)
        {
            bool DataReturn = false;
            try
            {
                var listThanhVien = (from l in db.THANHVIENHDQTs
                                     where l.MATD == matd
                                     select l);
                if (listThanhVien != null && listThanhVien.Count() > 0)
                {
                    DataReturn = true;
                    return DataReturn;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return DataReturn;
        }

        public BIEUQUYETYKIENController()
        {
            culture = new CultureInfo(1033);
        }

        // GET: /BIEUQUYETYKIEN/
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            ViewBag.Alert = TempData["Message"] + string.Empty;
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                QLDHCDEntities data = new QLDHCDEntities();
                List<BANGYKIEN> lst = new List<BANGYKIEN>();
                lst = (from l in db.BANGYKIENs where l.DHCD.ACTIVE == 1 select l).ToList();
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
                    lst = lst.Where(s => s.NOIDUNG.Contains(searchString)).ToList();
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

        // GET: /BIEUQUYETYKIEN/Details/5
        public ActionResult Details(int? id)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BANGYKIEN bangykien = db.BANGYKIENs.Find(id);
                if (bangykien == null)
                {
                    return HttpNotFound();
                }
                return View(bangykien);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
           
        }

        // GET: /BIEUQUYETYKIEN/Create
        public ActionResult Create()
        {

            if (HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty == "AdminQLDHCD"
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                DHCD dhcdh = db.DHCDs.Where(x=>x.ACTIVE==1).OrderBy(q=>q.thoiGian).First();
                BANGYKIEN bangyk = new BANGYKIEN();
                bangyk.MADH = dhcdh.MADH;
                bangyk.SLDONGY = 0;
                bangyk.NCPDONGY = 0;
                bangyk.NCPKHONGDONGY = 0;
                bangyk.NCPKHAC = 0;
                bangyk.SLKHONGDONGY = 0;
                bangyk.SLYKKHAC = 0;
                long tongslcp = db.CT_DHCD.Where(x=>x.MADH == dhcdh.MADH).Sum(q=>q.SLCP) ?? 0;
                bangyk.SOPHIEUPHATRA = db.CT_DHCD.Where(x => x.MADH == dhcdh.MADH).Count();
                bangyk.TUONGDUONGCOPHIEU = tongslcp;

                ViewBag.TenDH = dhcdh.TenDH;
                return View(bangyk);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
            
        }

        // POST: /BIEUQUYETYKIEN/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="MAYK,MADH,NOIDUNG,SLDONGY,SLKHONGDONGY,SLYKKHAC,SOPHIEUPHATRA,TUONGDUONGCOPHIEU")] BANGYKIEN bangykien)
        {
            if (ModelState.IsValid)
            {
                db.BANGYKIENs.Add(bangykien);
                db.SaveChanges();
                TempData["Message"] = "Tạo mới ý kiến biểu quyết thành công ";
                return RedirectToAction("Index");
            }

            ViewBag.MADH = new SelectList(db.DHCDs, "MADH", "TenDH", bangykien.MADH);
            return View(bangykien);
        }

        // GET: /BIEUQUYETYKIEN/Edit/5
        public ActionResult Edit(int? id)
        {
            if (HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty == "AdminQLDHCD"
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BANGYKIEN bangykien = db.BANGYKIENs.Find(id);
                if (bangykien == null)
                {
                    return HttpNotFound();
                }
                return View(bangykien);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }

           
        }

        // POST: /BIEUQUYETYKIEN/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="MAYK,MADH,NOIDUNG,SLDONGY,SLKHONGDONGY,SLYKKHAC,SOPHIEUPHATRA,TUONGDUONGCOPHIEU")] BANGYKIEN bangykien)
        {

            if (HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty == "AdminQLDHCD"
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                if (ModelState.IsValid)
                {
                    db.Entry(bangykien).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.MADH = new SelectList(db.DHCDs, "MADH", "TenDH", bangykien.MADH);
                return View(bangykien);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }

           
        }

        // GET: /BIEUQUYETYKIEN/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BANGYKIEN bangykien = db.BANGYKIENs.Find(id);
        //    if (bangykien == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(bangykien);
        //}

        //// POST: /BIEUQUYETYKIEN/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    BANGYKIEN bangykien = db.BANGYKIENs.Find(id);
        //    db.BANGYKIENs.Remove(bangykien);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
