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
    public class UyQuyenController : Controller
    {
        //
        // GET: /UyQuyen/
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            List<UYQUYEN> lst = new List<UYQUYEN>();
            lst = (from l in data.UYQUYENs
                   where l.NGUOICHUYEN.DHCD.ACTIVE==1
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
                lst = lst.Where(s => s.NGUOICHUYEN.CODONG.HoTen.Contains(searchString) || s.NGUOINHAN.CODONG.HoTen.Contains(searchString)).ToList();
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lst.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            UYQUYEN v = new UYQUYEN();
            PopulateDHDropDownList();
            PopulateNgChuyenDropDownList();
            PopulateNgNhanDropDownList();
            return View(v);
        }

        public ActionResult Insert([Bind(Include="MAUQ,MADH,MANGCHUYEN,MANGNHAN,SLUQ,THOIGIAN")] UYQUYEN uyquyen)
        {
            if (ModelState.IsValid)
            {
                QLDHCDEntities data = new QLDHCDEntities();
                data.UYQUYENs.Add(uyquyen);
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateDHDropDownList();
            PopulateNgChuyenDropDownList();
            PopulateNgNhanDropDownList();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QLDHCDEntities data = new QLDHCDEntities();
            UYQUYEN uyquyen = data.UYQUYENs.Find(id);
            if (uyquyen == null)
            {
                return HttpNotFound();
            }
            return View(uyquyen);
        }

        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    QLDHCDEntities data = new QLDHCDEntities();
        //    UYQUYEN uyquyen = data.UYQUYENs.Find(id);
        //    if (uyquyen == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    PopulateDHDropDownList();
        //    PopulateNgChuyenDropDownList();
        //    PopulateNgNhanDropDownList();
        //    return View(uyquyen);
        //}
        //public ActionResult Update([Bind(Include = "MAUQ,MADH,MANGCHUYEN,MANGNHAN,SLUQ,THOIGIAN")] UYQUYEN uyquyen)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        QLDHCDEntities data = new QLDHCDEntities();
        //        data.Entry(uyquyen).State = EntityState.Modified;
        //        data.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    PopulateDHDropDownList();
        //    PopulateNgChuyenDropDownList();
        //    PopulateNgNhanDropDownList();
        //    return RedirectToAction("Index");
        //}



        public void PopulateDHDropDownList(object selectedDH = null)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            var dhQuery = from d in data.DHCDs
                          where d.ACTIVE == 1
                          select d;
            ViewBag.MADH = new SelectList(dhQuery, "MADH", "TenDH", selectedDH);
        }
        public void PopulateNgChuyenDropDownList(object selectedCD = null)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            var cdQuery = from d in data.CT_DHCD
                          where d.DHCD.ACTIVE==1
                          select d;
            ViewBag.MANGCHUYEN = new SelectList(cdQuery, "MACD", "CODONG.HoTen", selectedCD);
        }
        public void PopulateNgNhanDropDownList(object selectedCD = null)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            var cdQuery = from d in data.CT_DHCD
                          where d.DHCD.ACTIVE == 1
                          select d;
            ViewBag.MANGNHAN = new SelectList(cdQuery, "MACD", "CODONG.HoTen", selectedCD);
        }
	}
}