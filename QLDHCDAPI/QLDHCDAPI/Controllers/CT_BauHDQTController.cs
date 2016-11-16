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
    public class CT_BauHDQTController : Controller
    {
        private QLDHCDEntities db = new QLDHCDEntities();
        DAO Dao = new DAO();
        CultureInfo culture = CultureInfo.CurrentCulture;

        public CT_BauHDQTController()
        {
            culture = new CultureInfo(1033);
        }

        // GET: /CT_BauHDQT/
        public ActionResult Index( string currentFilter, string searchString, int? page)
        {
            ViewBag.Alert = TempData["Message"] + string.Empty;
            ViewBag.ListAlert = TempData["listMess"];
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                
                List<CT_BAUHDQT> lst = new List<CT_BAUHDQT>();
                DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(q => q.thoiGian).First();

                lst = (from l in db.CT_BAUHDQT where l.THANHVIENHDQT.CT_DHCD.MADH == dhcd.MADH select l).ToList();
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

        public ActionResult BauThanhVien()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
              && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
              && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                string htdkOldTV = "HĐQT đại hội trước";
                DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(q => q.thoiGian).First();
                List<THANHVIENHDQT> listHDQT = (from l in db.THANHVIENHDQTs
                                                where l.CT_DHCD.MADH == dhcd.MADH && l.HINHTHUCBAU != htdkOldTV
                                                select l).ToList();
                string userName = HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty;
                USERCD CurrentUser = db.USERCDs.Where(x => x.USERNAME == userName).First();


                CT_BAUHDQT CurrentCT = new CT_BAUHDQT();
                CurrentCT.SLPHIEUBAU = 0;
                CurrentCT.THOIGIANBAU = DateTime.Now;
                CurrentCT.LAHOPLE = true;
                CurrentCT.HINHTHUCBAU = "Trực tiếp";

                long SoPhieuThuVao = db.CT_BAUHDQT.Where(x => x.CT_DHCD.DHCD.MADH == dhcd.MADH).Sum(q => q.SLPHIEUBAU);
                long SoPhieuHopLe = db.CT_BAUHDQT.Where(x => x.CT_DHCD.DHCD.MADH == dhcd.MADH && x.LAHOPLE == true).Sum(q => q.SLPHIEUBAU);
                long SoPhieuKhongHople = SoPhieuThuVao - SoPhieuHopLe;
                ViewBag.SLHopLe = SoPhieuHopLe;
                ViewBag.SLKhongHopLe = SoPhieuKhongHople;
                ViewBag.SoPhieuThuVao = SoPhieuThuVao;
                ViewBag.SoPhieuPhatRa = dhcd.SLCPPHATRA_HDQT;
                ViewBag.TichSo = listHDQT.Count;
                ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM;
                ViewBag.YearDH = dhcd.YEARDHCD;

                ListModel<CT_BAUHDQT> listReturn = new ListModel<CT_BAUHDQT>();

                //List<KeyValuePair<THANHVIENHDQT, CT_BAUHDQT>> listCT = new List<KeyValuePair<THANHVIENHDQT, CT_BAUHDQT>>();
                foreach (THANHVIENHDQT v in listHDQT)
                {
                    CT_BAUHDQT Row = new CT_BAUHDQT();
                    Row.SLPHIEUBAU = 0;
                    Row.THOIGIANBAU = DateTime.Now;
                    Row.LAHOPLE = true;
                    Row.HINHTHUCBAU = "Trực tiếp";
                    Row.MAHDQT = v.MATD;
                    Row.THANHVIENHDQT = v;
                    listReturn.Items.Add(Row);
                }
                return View(listReturn);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Khong co Quyen truy cap");
            }
        }

        [HttpPost]
        public ActionResult BauThanhVien(ListModel<CT_BAUHDQT> listReturn)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
              && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
              && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                List<string> Mess = new List<string>();
                foreach (CT_BAUHDQT v in listReturn.Items)
                {
                    THANHVIENHDQTController tvhdqtController = new THANHVIENHDQTController();
                    try
                    {
                        if (tvhdqtController.checkThanhVien(v.MAHDQT))
                        {
                            ObjectParameter myCheck = new ObjectParameter("CHECKSUCCESS", 0);
                            db.CT_BAUHDQT_INSERT(v.MAHDQT, v.MANGUOIBAU, v.SLPHIEUBAU, v.HINHTHUCBAU, v.LAHOPLE, myCheck);
                            if(myCheck.Value.ToString() == "1")
                            {
                                Mess.Add("Bầu thành viên " + v.MAHDQT + " thành công ");
                            }
                            else
                            {
                                Mess.Add("Bầu thành viên " + v.MAHDQT + "không thành công ");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Mess.Add("Bầu thành viên " + v.MAHDQT + " không thành công ");
                    }
                }

                TempData["listMess"] = Mess;
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(401, "Khong co Quyen truy cap");
            }
        }

        // GET: /CT_BauHDQT/Details/5
        public ActionResult Details(int? id)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
              && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CT_BAUHDQT ct_bauhdqt = db.CT_BAUHDQT.Find(id);
                if (ct_bauhdqt == null)
                {
                    return HttpNotFound();
                }
                return View(ct_bauhdqt);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Khong Co quyen");
            }
            
        }

        //// GET: /CT_BauHDQT/Create
        //public ActionResult Create()
        //{
        //    ViewBag.MAHDQT = new SelectList(db.CT_DHCD, "MATD", "MADH");
        //    ViewBag.MANGUOIBAU = new SelectList(db.CT_DHCD, "MATD", "MADH");
        //    return View();
        //}

        //// POST: /CT_BauHDQT/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include="MACT,MAHDQT,MANGUOIBAU,SLPHIEUBAU,HINHTHUCBAU,THOIGIANBAU")] CT_BAUHDQT ct_bauhdqt)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.CT_BAUHDQT.Add(ct_bauhdqt);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.MAHDQT = new SelectList(db.CT_DHCD, "MATD", "MADH", ct_bauhdqt.MAHDQT);
        //    ViewBag.MANGUOIBAU = new SelectList(db.CT_DHCD, "MATD", "MADH", ct_bauhdqt.MANGUOIBAU);
        //    return View(ct_bauhdqt);
        //}

        //// GET: /CT_BauHDQT/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CT_BAUHDQT ct_bauhdqt = db.CT_BAUHDQT.Find(id);
        //    if (ct_bauhdqt == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.MAHDQT = new SelectList(db.CT_DHCD, "MATD", "MADH", ct_bauhdqt.MAHDQT);
        //    ViewBag.MANGUOIBAU = new SelectList(db.CT_DHCD, "MATD", "MADH", ct_bauhdqt.MANGUOIBAU);
        //    return View(ct_bauhdqt);
        //}

        //// POST: /CT_BauHDQT/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="MACT,MAHDQT,MANGUOIBAU,SLPHIEUBAU,HINHTHUCBAU,THOIGIANBAU")] CT_BAUHDQT ct_bauhdqt)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(ct_bauhdqt).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.MAHDQT = new SelectList(db.CT_DHCD, "MATD", "MADH", ct_bauhdqt.MAHDQT);
        //    ViewBag.MANGUOIBAU = new SelectList(db.CT_DHCD, "MATD", "MADH", ct_bauhdqt.MANGUOIBAU);
        //    return View(ct_bauhdqt);
        //}

        //// GET: /CT_BauHDQT/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CT_BAUHDQT ct_bauhdqt = db.CT_BAUHDQT.Find(id);
        //    if (ct_bauhdqt == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ct_bauhdqt);
        //}

        //// POST: /CT_BauHDQT/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    CT_BAUHDQT ct_bauhdqt = db.CT_BAUHDQT.Find(id);
        //    db.CT_BAUHDQT.Remove(ct_bauhdqt);
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
