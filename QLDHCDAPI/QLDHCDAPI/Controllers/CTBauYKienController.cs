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

namespace QLDHCDAPI.Controllers
{
    public class CTBauYKienController : Controller
    {
        //
        // GET: /CTBauYKien/
        public ActionResult Index(int mayk, string currentFilter, string searchString, int? page)
        {
            if (mayk ==null || mayk<=0 )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.pamayk = mayk;
            QLDHCDEntities data = new QLDHCDEntities();
            List<CTBQYKIEN> lst = new List<CTBQYKIEN>();
            lst = (from l in data.CTBQYKIENs
                   where l.MAYK==mayk
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

            BANGYKIEN temp = new BANGYKIEN();
            temp = (from v in data.BANGYKIENs
                    where v.MAYK== mayk
                    select v).First();
            ViewBag.noiDungYK = temp.NOIDUNG;
            return View(lst.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Details(int? mayk, int macd, string madh)
        {
            if (mayk == null || macd == null || String.IsNullOrEmpty(madh))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QLDHCDEntities data = new QLDHCDEntities();
            CTBQYKIEN dd = (from v in data.CTBQYKIENs
                            where v.MAYK == mayk && v.MADH == madh && v.MACD == macd
                            select v).First();
            if (dd == null)
            {
                return HttpNotFound();
            }
            return View(dd);
        }
        // GET: /aaa/Create
        public ActionResult Create(int mayk)
        {
            CTBQYKIEN d = new CTBQYKIEN();
            QLDHCDEntities data = new QLDHCDEntities();

            d.MAYK = mayk;
            ViewBag.pamayk = mayk;
            PopulateDHDropDownList();
            PopulateNGDiBauDropDownList();
            return View(d);
        }

        // POST: /aaa/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MAYK,MADH,MACD,LUACHON,NOIDUNGYKKHAC,THOIGIANBAU,SLPHIEUBAU")] CTBQYKIEN f)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    QLDHCDEntities data = new QLDHCDEntities();
                    data.CTBQYKIENs.Add(f);
                    data.SaveChanges();

                    ViewBag.check = 1;
                    ViewBag.checkString = "Thêm thành công !!!";

                    ViewBag.pamayk = f.MAYK;
                    PopulateDHDropDownList();
                    PopulateNGDiBauDropDownList();
                    return View();
                }
            }
            catch
            {
                ViewBag.check = 0;
                ViewBag.checkString = "Thêm thất bại !!!";
            }

            ViewBag.pamayk = f.MAYK;
            PopulateDHDropDownList();
            PopulateNGDiBauDropDownList();
            return View(f);
        }

        public void PopulateDHDropDownList(object selectedDH = null)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            var dhQuery = from d in data.DHCDs
                          where d.ACTIVE == 1
                          select d;
            ViewBag.MADH = new SelectList(dhQuery, "MADH", "TenDH", selectedDH);
        }
        public void PopulateNGDiBauDropDownList(object selectedCD = null)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            var cdQuery = from d in data.CT_DHCD
                          select d;
            ViewBag.MACD = new SelectList(cdQuery, "MACD", "CODONG.HoTen", selectedCD);
        }

	}
}