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
    public class CT_BQPHIEUController : Controller
    {
        private QLDHCDEntities db = new QLDHCDEntities();


        DAO Dao = new DAO();
        CultureInfo culture = CultureInfo.CurrentCulture;

        public CT_BQPHIEUController()
        {
            culture = new CultureInfo(1033);
        }

        // GET: /CT_BQPHIEU/
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            ViewBag.Alert = TempData["Message"] + string.Empty;
            ViewBag.ListAlert = TempData["listMess"];
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {

                List<CT_YKIEN_BQPHIEU> lst = new List<CT_YKIEN_BQPHIEU>();
                DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(q => q.thoiGian).First();

                lst = (from l in db.CT_YKIEN_BQPHIEU where l.BANGYKIENBQPHIEU.DHCD.MADH == dhcd.MADH select l).ToList();
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
                    lst = lst.Where(s => s.BANGYKIENBQPHIEU.NOIDUNG.Contains(searchString)).ToList();
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

        // GET: /CT_BQPHIEU/Details/5
        public ActionResult Details(int? id)
        {

            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CT_YKIEN_BQPHIEU ct_ykien_bqphieu = db.CT_YKIEN_BQPHIEU.Find(id);
                if (ct_ykien_bqphieu == null)
                {
                    return HttpNotFound();
                }
                return View(ct_ykien_bqphieu);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }

        }

        public ActionResult BieuQuyet()
        {

            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
            {
                try
                {
                    DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(q => q.thoiGian).First();

                    ListModel<CT_YKIEN_BQPHIEU> ListReturn = new ListModel<CT_YKIEN_BQPHIEU>();
                    List<BANGYKIENBQPHIEU> listYKien = db.BANGYKIENBQPHIEUx.Where(x => x.MADH == dhcd.MADH).ToList();
                    if (listYKien == null || listYKien.Count <= 0)
                    {
                        TempData["Message"] = "Chưa có ý kiến được khởi tạo";
                        return RedirectToAction("Index");
                    }
                    foreach (BANGYKIENBQPHIEU item in listYKien)
                    {
                        CT_YKIEN_BQPHIEU ct = new CT_YKIEN_BQPHIEU();
                        ct.MAYK = item.MAYK;
                        ct.LUACHON = 1;
                        ct.LAHOPLE = true;
                        ct.THOIGIANBAU = DateTime.Now;
                        ct.BANGYKIENBQPHIEU = item;
                        ListReturn.Items.Add(ct);
                    }

                    int slThuVao = db.BANGYKIENBQPHIEUx.Where(x => x.MADH == dhcd.MADH).Max(q => q.SL_THUVAO);
                    long SLCPThuVao = db.BANGYKIENBQPHIEUx.Where(x => x.MADH == dhcd.MADH).Max(q => q.SLCP_THUVAO);
                    int slHople = db.BANGYKIENBQPHIEUx.Where(x => x.MADH == dhcd.MADH).Max(q => q.SL_HOPLE);
                    long SLCPHopLe = db.BANGYKIENBQPHIEUx.Where(x => x.MADH == dhcd.MADH).Max(q => q.SLCP_HOPLE);
                    int slKhongHople = db.BANGYKIENBQPHIEUx.Where(x => x.MADH == dhcd.MADH).Max(q => q.SL_KHONG_HOPLE);
                    long SLCPKhongHopLe = db.BANGYKIENBQPHIEUx.Where(x => x.MADH == dhcd.MADH).Max(q => q.SLCP_KHONG_HOPLE);
                    int slPhatRa = db.BANGYKIENBQPHIEUx.Where(x => x.MADH == dhcd.MADH).Max(q => q.SL_PHATRA);
                    long SLCPPhatRa = db.BANGYKIENBQPHIEUx.Where(x => x.MADH == dhcd.MADH).Max(q => q.SLCP_PHATRA);

                    ViewBag.slThuVao = slThuVao;
                    ViewBag.SLCPThuVao = SLCPThuVao;
                    ViewBag.slHople = slHople;
                    ViewBag.SLCPHopLe = SLCPHopLe;
                    ViewBag.slKhongHople = slKhongHople;
                    ViewBag.SLCPKhongHopLe = SLCPKhongHopLe;
                    ViewBag.slPhatRa = slPhatRa;
                    ViewBag.SLCPPhatRa = SLCPPhatRa;
                    ViewBag.DHCD = dhcd;
                    ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM + string.Empty;
                    return View(ListReturn);
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(400, "Bad request" + ex.Message);
                }
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }

        [HttpPost]
        public ActionResult BieuQuyet(ListModel<CT_YKIEN_BQPHIEU> listReturn)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
            {
                try
                {
                    if (listReturn == null || listReturn.Items.Count <= 0)
                    {
                        ModelState.AddModelError("", "Đã có lỗi xảy ra, không thể biểu quyết thành công");
                    }
                    int i = 0;
                    if (ModelState.IsValid)
                    {
                        foreach (CT_YKIEN_BQPHIEU v in listReturn.Items)
                        {
                            v.THOIGIANBAU = DateTime.Now;
                            v.BANGYKIENBQPHIEU = null;
                            db.CT_YKIEN_BQPHIEU.Add(v);
                            try
                            {
                                db.SaveChanges();
                                i++;
                            }
                            catch
                            {

                            }
                        }
                        if (i == listReturn.Items.Count)
                        {
                            TempData["Message"] = "Biểu quyết ý kiến bằng phiếu thành công";
                            return RedirectToAction("Index", "BIEUQUYETPHIEU");
                        }

                    }

                    return BieuQuyet();


                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(400, "Bad request" + ex.Message);
                }
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }


        //// GET: /CT_BQPHIEU/Create
        //public ActionResult Create()
        //{
        //    ViewBag.MAYK = new SelectList(db.BANGYKIENBQPHIEUx, "MAYK", "MADH");
        //    ViewBag.MATD = new SelectList(db.CT_DHCD, "MATD", "MADH");
        //    return View();
        //}

        //// POST: /CT_BQPHIEU/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include="ID,MAYK,MATD,LUACHON,THOIGIANBAU,LAHOPLE")] CT_YKIEN_BQPHIEU ct_ykien_bqphieu)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.CT_YKIEN_BQPHIEU.Add(ct_ykien_bqphieu);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.MAYK = new SelectList(db.BANGYKIENBQPHIEUx, "MAYK", "MADH", ct_ykien_bqphieu.MAYK);
        //    ViewBag.MATD = new SelectList(db.CT_DHCD, "MATD", "MADH", ct_ykien_bqphieu.MATD);
        //    return View(ct_ykien_bqphieu);
        //}

        //// GET: /CT_BQPHIEU/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CT_YKIEN_BQPHIEU ct_ykien_bqphieu = db.CT_YKIEN_BQPHIEU.Find(id);
        //    if (ct_ykien_bqphieu == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.MAYK = new SelectList(db.BANGYKIENBQPHIEUx, "MAYK", "MADH", ct_ykien_bqphieu.MAYK);
        //    ViewBag.MATD = new SelectList(db.CT_DHCD, "MATD", "MADH", ct_ykien_bqphieu.MATD);
        //    return View(ct_ykien_bqphieu);
        //}

        //// POST: /CT_BQPHIEU/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="ID,MAYK,MATD,LUACHON,THOIGIANBAU,LAHOPLE")] CT_YKIEN_BQPHIEU ct_ykien_bqphieu)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(ct_ykien_bqphieu).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.MAYK = new SelectList(db.BANGYKIENBQPHIEUx, "MAYK", "MADH", ct_ykien_bqphieu.MAYK);
        //    ViewBag.MATD = new SelectList(db.CT_DHCD, "MATD", "MADH", ct_ykien_bqphieu.MATD);
        //    return View(ct_ykien_bqphieu);
        //}

        // GET: /CT_BQPHIEU/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
             && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
             && HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
            {
                CT_YKIEN_BQPHIEU ct_ykien_bqphieu = db.CT_YKIEN_BQPHIEU.Find(id);
                if (ct_ykien_bqphieu == null)
                {
                    return HttpNotFound();
                }
                return View(ct_ykien_bqphieu);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }

        // POST: /CT_BQPHIEU/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
            && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
            && HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
            {
                CT_YKIEN_BQPHIEU ct_ykien_bqphieu = db.CT_YKIEN_BQPHIEU.Find(id);
                List<CT_YKIEN_BQPHIEU> listRemove = db.CT_YKIEN_BQPHIEU.Where(x=>x.MATD == ct_ykien_bqphieu.MATD).ToList();
                foreach(var v in listRemove)
                {
                    db.CT_YKIEN_BQPHIEU.Remove(v);
                }
                try
                {
                    db.SaveChanges();
                    TempData["Message"] = "Xóa chi tiết biểu quyết thành công, nếu còn chi tiết biểu quyết có";
                }
                catch
                {
                    TempData["Message"] = "Xóa chi tiết biểu quyết không thành công";
                }
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
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
