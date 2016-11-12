using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLDHCDAPI.Models;
using PagedList;
using PagedList.Mvc;
using System.Net;
namespace QLDHCDAPI.Controllers
{
    public class BangBauBKSController : Controller
    {
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            List<BANGBAUBK> lst = new List<BANGBAUBK>();
            lst = (from l in data.BANGBAUBKS
                   where l.CT_DHCD.DHCD.ACTIVE == 1
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
                lst = lst.Where(s => s.CT_DHCD.CODONG.HoTen.Contains(searchString)).ToList();
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lst.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            BANGBAUBK cd = new BANGBAUBK();
            QLDHCDEntities data = new QLDHCDEntities();
            PopulateCDDropDownList();
            PopulateDHDropDownList();
            return View(cd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MADH,MACD,HINHTHUCBAU,SLPHIEUBAU")] BANGBAUBK f)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    QLDHCDEntities data = new QLDHCDEntities();
                    data.BANGBAUBKS.Add(f);
                    data.SaveChanges();
                    ViewBag.check = 1;
                    ViewBag.checkString = "Thêm thành công ";
                }
            }
            catch
            {
                ViewBag.checkString = "Thêm thất bại";
                ViewBag.check = 0;

            }
            PopulateCDDropDownList();
            PopulateDHDropDownList();
            return View(f);
        }

        public ActionResult Details(string madh, int macd)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            BANGBAUBK dd = (from d in data.BANGBAUBKS
                              where d.MADH == madh && d.MACD == macd
                              select d).First();
            PopulateCDDropDownList(dd.MACD);
            PopulateDHDropDownList(dd.MADH);
            return View(dd);
        }
        public ActionResult Delete(string madh, int macd)
        {
            if (madh == null || macd == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QLDHCDEntities data = new QLDHCDEntities();
            BANGBAUBK dd = (from v in data.BANGBAUBKS
                              where v.MADH == madh && v.MACD == macd
                              select v).First();
            if (dd == null)
            {
                return HttpNotFound();
            }
            return View(dd);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string madh, int macd)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            BANGBAUBK dd = (from v in data.BANGBAUBKS
                                       where v.MACD == macd && v.MADH == madh
                                       select v).First();
            data.BANGBAUBKS.Remove(dd);
            data.SaveChanges();
            return RedirectToAction("Index");
        }






        public void PopulateDHDropDownList(object selectedDH = null)
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
            var cdQuery = from d in data.CT_DHCD
                          where d.DHCD.ACTIVE == 1
                          select d;
            ViewBag.MACD = new SelectList(cdQuery, "MACD", "CODONG.HoTen", selectedCD);
        }
	}
}