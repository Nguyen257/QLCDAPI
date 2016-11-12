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
    public class CTBauBKSController : Controller
    {
        //
        // GET: /CTBauBKS/
        public ActionResult Index(string madh, int macd, string currentFilter, string searchString, int? page)
        {
            if (String.IsNullOrEmpty(madh) || macd == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.madh = madh;
            ViewBag.macd = macd;
            QLDHCDEntities data = new QLDHCDEntities();
            List<CT_BAUBKS> lst = new List<CT_BAUBKS>();
            lst = (from l in data.CT_BAUBKS
                   where l.MADH == madh && l.MANGDUOCBAU == macd
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

            BANGBAUBK temp = new BANGBAUBK();
            temp = (from v in data.BANGBAUBKS
                    where v.MADH == madh && v.MACD == macd
                    select v).First();
            ViewBag.madh = madh;
            ViewBag.macd = macd;
            ViewBag.tenUngVien = temp.CT_DHCD.CODONG.HoTen;
            return View(lst.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create(string madh, int macd)
        {
            if (String.IsNullOrEmpty(madh))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CT_BAUBKS v = new CT_BAUBKS();
            PopulateDHDropDownList(madh);
            PopulateNGDcBauDropDownList(macd);
            PopulateNGDiBauDropDownList(macd);

            ViewBag.pamadh = madh;
            ViewBag.pamacd = macd;
            return View(v);
        }
        public ActionResult Insert([Bind(Include = "MACT,MADH,MANGDUOCBAU,MANGDIBAU,SLPHIEUBAU,HINHTHUCBAU,THOIGIANBAU")] CT_BAUBKS f)
        {
            if (f == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                if (ModelState.IsValid)
                {
                    QLDHCDEntities data = new QLDHCDEntities();
                    data.CT_BAUBKS.Add(f);
                    data.SaveChanges();
                    ViewBag.check = 1;
                    ViewBag.checkString = "Thêm thành công";
                    PopulateDHDropDownList();
                    PopulateNGDcBauDropDownList(f.MANGDUOCBAU);
                    PopulateNGDiBauDropDownList(f.MANGDUOCBAU);

                    ViewBag.pamadh = f.MADH;
                    ViewBag.pamacd = f.MANGDUOCBAU;
                    return View("Create", f);
                }
            }
            catch
            {
                ViewBag.check = 0;
                ViewBag.checkString = " Thêm thất bại";
            }
            PopulateDHDropDownList();
            PopulateNGDcBauDropDownList(f.MANGDUOCBAU);
            PopulateNGDiBauDropDownList(f.MANGDUOCBAU);

            ViewBag.pamadh = f.MADH;
            ViewBag.pamacd = f.MANGDUOCBAU;
            return View("Create", f);
        }


        public void PopulateDHDropDownList(object selectedDH = null)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            var dhQuery = from d in data.DHCDs
                          where d.ACTIVE == 1
                          select d;
            ViewBag.MADH = new SelectList(dhQuery, "MADH", "TenDH", selectedDH);
        }
        public void PopulateNGDcBauDropDownList(int? macd, object selectedCD = null)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            var cdQuery = from d in data.CT_DHCD
                          where d.MACD == macd
                          select d;
            ViewBag.MANGDUOCBAU = new SelectList(cdQuery, "MACD", "CODONG.HoTen", selectedCD);
        }
        public void PopulateNGDiBauDropDownList(int? macd, object selectedCD = null)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            var cdQuery = from d in data.CT_DHCD
                          where d.MACD != macd
                          select d;
            ViewBag.MANGDIBAU = new SelectList(cdQuery, "MACD", "CODONG.HoTen", selectedCD);
        }
	}
}