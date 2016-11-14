﻿using System;
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
        private QLDHCDEntities db = new QLDHCDEntities();

        // GET: /ThamGiaDH/
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                              && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
                {
                    ViewBag.Alert = TempData["Message"] + string.Empty;
                    QLDHCDEntities data = new QLDHCDEntities();
                    List<CT_DHCD> lst = new List<CT_DHCD>();
                    List<CT_DHCD> CurrentCT_DHCD = db.CT_DHCD.Where(q => q.DHCD.ACTIVE == 1).ToList(); ;

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
                        if (CurrentCT_DHCD != null && CurrentCT_DHCD.Count >= 0)
                        {
                            lst = CurrentCT_DHCD.Where(q => q.CODONG.HoTen.Contains(searchString) || q.DHCD.TenDH.Contains(searchString)).ToList();
                        }
                    }
                    else
                    {
                        lst = CurrentCT_DHCD;
                    }
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return View(lst.ToPagedList(pageNumber, pageSize));

                }
            }
            return new HttpStatusCodeResult(401);
        }

        // GET: /ThamGiaDH/Details/5
        public ActionResult Details(string matd)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                     && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
                {
                    if (string.IsNullOrWhiteSpace(matd))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    CT_DHCD ct_dhcd = db.CT_DHCD.Find(matd);
                    if (ct_dhcd == null)
                    {
                        return HttpNotFound();
                    }
                    return View(ct_dhcd);
                }
                else
                {
                    if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "User")
                    {
                        List<int> lCT_DHCD = (from td in db.CT_DHCD where td.MATD == matd select td.MACD).ToList();
                        int macd = (lCT_DHCD != null && lCT_DHCD.Count > 0) ? lCT_DHCD.First() : 0;
                        string UserName = db.USERCDs.Where(q => q.MACD == macd).First().USERNAME;
                        if (UserName == HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                        {
                            CT_DHCD ct_dhcd = db.CT_DHCD.Find(matd);
                            if (ct_dhcd == null)
                            {
                                return HttpNotFound();
                            }
                            return View(ct_dhcd);
                        }
                    }
                }
            }
            return new HttpStatusCodeResult(401);

        }

        // GET: /ThamGiaDH/Create
        public ActionResult Create()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                      && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
                {
                    CT_DHCD ct = new CT_DHCD();
                    DHCD dhcd = db.DHCDs.Where(q => q.ACTIVE == 1).ToList().First();
                    ViewBag.MADH = dhcd.MADH + string.Empty;
                    ViewBag.TenDH = dhcd.TenDH + string.Empty;
                    ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM + string.Empty;

                    return View(ct);
                }
            }
            return new HttpStatusCodeResult(401);

        }

        // POST: /ThamGiaDH/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MATD,MADH,MACD,SLCP,SLDAUQ,SLDCUQ,HTDK,SLCPSAUCUNG")] CT_DHCD ct_dhcd)
        {
            if (ModelState.IsValid)
            {
                ct_dhcd.SLCP = (ct_dhcd.SLCP.HasValue) ? (ct_dhcd.SLCP.Value) : 0;
                ct_dhcd.SLDAUQ = (ct_dhcd.SLDAUQ.HasValue) ? (ct_dhcd.SLDAUQ.Value) : 0;
                ct_dhcd.SLDCUQ = (ct_dhcd.SLDCUQ.HasValue) ? (ct_dhcd.SLDCUQ.Value) : 0;
                ct_dhcd.SLCPSAUCUNG = ct_dhcd.SLCP - ct_dhcd.SLDAUQ + ct_dhcd.SLDCUQ;
                db.CT_DHCD.Add(ct_dhcd);
                db.SaveChanges();
                TempData["Message"] = "Đăng ký tham dự đại hội thành công";
                return RedirectToAction("Index");
            }

            ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen", ct_dhcd.MACD);
            ViewBag.MADH = new SelectList(db.DHCDs, "MADH", "TenDH", ct_dhcd.MADH);
            ViewBag.MATD = new SelectList(db.THANHVIENBKS, "MATD", "HINHTHUCBAU", ct_dhcd.MATD);
            ViewBag.MATD = new SelectList(db.THANHVIENHDQTs, "MATD", "HINHTHUCBAU", ct_dhcd.MATD);
            return View(ct_dhcd);
        }

        // GET: /ThamGiaDH/Edit/5
        public ActionResult Edit(string matd)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                      && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
                {
                    if (string.IsNullOrWhiteSpace(matd))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    CT_DHCD ct_dhcd = db.CT_DHCD.Find(matd);
                    if (ct_dhcd == null)
                    {
                        return HttpNotFound();
                    }

                    return View(ct_dhcd);
                }
            }
            return new HttpStatusCodeResult(401);

        }

        // POST: /ThamGiaDH/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MATD,MADH,MACD,SLCP,SLDAUQ,SLDCUQ,HTDK,SLCPSAUCUNG")] CT_DHCD ct_dhcd)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                     && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            ct_dhcd.SLCP = (ct_dhcd.SLCP.HasValue) ? (ct_dhcd.SLCP.Value) : 0;
                            ct_dhcd.SLDAUQ = (ct_dhcd.SLDAUQ.HasValue) ? (ct_dhcd.SLDAUQ.Value) : 0;
                            ct_dhcd.SLDCUQ = (ct_dhcd.SLDCUQ.HasValue) ? (ct_dhcd.SLDCUQ.Value) : 0;
                            ct_dhcd.SLCPSAUCUNG = ct_dhcd.SLCP - ct_dhcd.SLDAUQ + ct_dhcd.SLDCUQ;
                            db.Entry(ct_dhcd).State = EntityState.Modified;
                            db.SaveChanges();
                            TempData["Message"] = "Chỉnh sửa tham dự đại hội thành công";
                            return RedirectToAction("Index");
                        }
                        return View(ct_dhcd);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }
            return new HttpStatusCodeResult(401);
        }

        // GET: /ThamGiaDH/Delete/5
        public ActionResult Delete(string matd)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                      && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
                {
                    if (string.IsNullOrWhiteSpace(matd))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    CT_DHCD ct_dhcd = db.CT_DHCD.Find(matd);
                    if (ct_dhcd == null)
                    {
                        return HttpNotFound();
                    }
                    return View(ct_dhcd);
                }
            }
            return new HttpStatusCodeResult(401);

        }

        // POST: /ThamGiaDH/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string matd)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                      && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
                {
                    if (string.IsNullOrWhiteSpace(matd))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    CT_DHCD ct_dhcd = db.CT_DHCD.Find(matd);
                    db.CT_DHCD.Remove(ct_dhcd);
                    db.SaveChanges();
                    TempData["Message"] = "Xóa tham dự đại hội thành công";
                    return RedirectToAction("Index");
                }
            }
            return new HttpStatusCodeResult(401);

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
