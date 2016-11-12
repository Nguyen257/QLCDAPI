using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLDHCDAPI.Models;
using PagedList;
using PagedList.Mvc;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Data.Entity;

namespace QLDHCDAPI.Controllers
{
    public class ThamGiaDHController : Controller
    {
        //
        // GET: /ThamGiaDH/
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            List<CT_DHCD> lst = new List<CT_DHCD>();
            lst = (from l in data.CT_DHCD
                   where l.DHCD.ACTIVE==1
                   select l).ToList();

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
                lst = lst.Where(s => s.DHCD.TenDH.Contains(searchString) || s.CODONG.HoTen.Contains(searchString)).ToList();
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lst.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            CT_DHCD cd = new CT_DHCD();
            PopulateDHDropDownList();
            PopulateCDDropDownList();
            return View(cd);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert([Bind(Include ="MADH,MACD,SLCP,SLDAUQ,SLDCUQ,HTDK,SLCPSAUCUNG,QUYENBAUCU")] CT_DHCD f)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    QLDHCDEntities data = new QLDHCDEntities();
                    data.CT_DHCD.Add(f);
                    data.SaveChanges();
                    ViewBag.kq = "Đã thêm cổ đông thành công !!!";
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                ViewBag.kq = "Thêm cổ đông thất bại !!!";
            }
            PopulateCDDropDownList(f.MACD);
            PopulateDHDropDownList(f.MADH);
            return View("InsertResult");
        }
        public ActionResult Details(string mdh,int mcd)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            CT_DHCD dd = (from d in data.CT_DHCD
                          where d.MACD == mcd && d.MADH == mdh
                          select d).First();
            PopulateCDDropDownList(dd.MACD);
            PopulateDHDropDownList(dd.MADH);
            return View(dd);
        }
        public ActionResult Edit(string mdh ,int mcd)
        {
            
            if (mdh == null && mcd == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QLDHCDEntities data = new QLDHCDEntities();
            CT_DHCD ff = (from d in data.CT_DHCD
                          where d.MACD == mcd && d.MADH == mdh
                          select d).First();
            if (ff == null)
            {
                return HttpNotFound();
            }
            PopulateDHDropDownList(ff.MADH);
            PopulateCDDropDownList(ff.MACD);
            return View(ff);
        }

        public ActionResult Update([Bind(Include = "MADH,MACD,SLCP,SLDAUQ,SLDCUQ,HTDK,SLCPSAUCUNG,QUYENBAUCU")]CT_DHCD f)
        {
            QLDHCDEntities db = new QLDHCDEntities();
            if (ModelState.IsValid)
            {
                db.Entry(f).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string mdh, int mcd)
        {
            QLDHCDEntities db = new QLDHCDEntities();
            CT_DHCD ct_dhcd = (from d in db.CT_DHCD
                               where d.MADH == mdh && d.MACD == mcd
                               select d).First();
            db.CT_DHCD.Remove(ct_dhcd);
            db.SaveChanges();
            return RedirectToAction("Index");
        }








        public void PopulateDHDropDownList (object selectedDH=null)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            var dhQuery = from d in data.DHCDs
                          where d.ACTIVE == 1
                          select d;
            ViewBag.MADH = new SelectList(dhQuery, "MADH", "TenDH", selectedDH);
        }
        public void PopulateCDDropDownList(object selectedCD = null)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            var cdQuery = from d in data.CODONGs
                          select d;
            ViewBag.MACD = new SelectList(cdQuery, "MACD", "HoTen", selectedCD);
        }
	}
}