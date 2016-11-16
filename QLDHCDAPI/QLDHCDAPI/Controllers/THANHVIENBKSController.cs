﻿using System;
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
    public class THANHVIENBKSController : Controller
    {
        private QLDHCDEntities db = new QLDHCDEntities();
        DAO Dao = new DAO();
        CultureInfo culture = CultureInfo.CurrentCulture;

        public THANHVIENBKSController()
        {
            culture = new CultureInfo(1033);
        }

        // GET: /THANHVIENHDQT/
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            ViewBag.Alert = TempData["Message"] + string.Empty;
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                QLDHCDEntities data = new QLDHCDEntities();
                List<THANHVIENBK> lst = new List<THANHVIENBK>();
                lst = (from l in db.THANHVIENBKS where l.CT_DHCD.DHCD.ACTIVE == 1 select l).ToList();
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
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }

        // GET: /THANHVIENHDQT/Details/5
        public ActionResult Details(string id)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                THANHVIENBK thanhvienhdqt = db.THANHVIENBKS.Find(id);
                if (thanhvienhdqt == null)
                {
                    return HttpNotFound();
                }
                return View(thanhvienhdqt);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }


        }

        public ActionResult Create()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                try
                {
                    DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(q => q.thoiGian).First();

                    ViewBag.MaDH = dhcd.MADH;
                    ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM;
                    THANHVIENBK tvhdqt = new THANHVIENBK();
                    tvhdqt.LACHUTICH = false;
                    tvhdqt.LASUCCESS = false;
                    tvhdqt.SLPHIEUBAU = 0;

                    return View(tvhdqt);
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(400, "Khong co DHCD dang Active");
                }
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }

        }

        // POST: /Temp/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MATD,HINHTHUCBAU,SLPHIEUBAU,THANHVIENTYPE,LACHUTICH,LASUCCESS")] THANHVIENBK thanhvienbks)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
               && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
               && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                try
                {
                    DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(q => q.thoiGian).First();
                    if (ModelState.IsValid)
                    {
                        var ListChecktvHDQT = db.THANHVIENBKS.Where(x => x.MATD == thanhvienbks.MATD);
                        if (ListChecktvHDQT != null && ListChecktvHDQT.Count() > 0)
                        {
                            ModelState.AddModelError("", "Đã có thành viên này trong bầu BKS");
                            ViewBag.MaDH = dhcd.MADH;
                            ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM;
                            return View(thanhvienbks);
                        }
                        else
                        {
                            db.THANHVIENBKS.Add(thanhvienbks);
                            db.SaveChanges();
                            TempData["Message"] = "Thêm ứng viên vào bầu HĐQT thành công";
                            return RedirectToAction("Index");
                        }


                    }

                    ViewBag.MaDH = dhcd.MADH;
                    ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM;
                    return View(thanhvienbks);
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(400, "Khong co DHCD dang Active");
                }
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }



        }


        // GET: /THANHVIENHDQT/Edit/5
        public ActionResult Edit(string id)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                THANHVIENBK thanhvienhdqt = db.THANHVIENBKS.Find(id);
                if (thanhvienhdqt == null)
                {
                    return HttpNotFound();
                }
                ViewBag.MATD = thanhvienhdqt.MATD;
                return View(thanhvienhdqt);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }



        }

        // POST: /THANHVIENHDQT/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MATD,HINHTHUCBAU,SLPHIEUBAU,THANHVIENTYPE,LACHUTICH,LASUCCESS")] THANHVIENBK thanhvienhdqt)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                if (ModelState.IsValid)
                {
                    db.Entry(thanhvienhdqt).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Message"] = "Chỉnh sửa HĐQT thành công";

                    return RedirectToAction("Index");
                }
                ViewBag.MATD = thanhvienhdqt.MATD;
                return View(thanhvienhdqt);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }

        }

        // GET: /THANHVIENHDQT/Delete/5


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
