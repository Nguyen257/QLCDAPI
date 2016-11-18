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
    public class BIEUQUYETPHIEUController : Controller
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

        public BIEUQUYETPHIEUController()
        {
            culture = new CultureInfo(1033);
        }

        // GET: /BIEUQUYETPHIEU/
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            ViewBag.Alert = TempData["Message"] + string.Empty;
            ViewBag.ListAlert = TempData["listMess"];
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                QLDHCDEntities data = new QLDHCDEntities();
                List<BANGYKIENBQPHIEU> lst = new List<BANGYKIENBQPHIEU>();
                DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(q => q.thoiGian).First();
                lst = (from l in db.BANGYKIENBQPHIEUx where l.MADH == dhcd.MADH select l).ToList();
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

        public ActionResult UpdateSLCPPhatra(string Role)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
            {
                try
                {
                    DHCD dhcdh = db.DHCDs.Where(x => x.ACTIVE == 1).OrderBy(q => q.thoiGian).First();
                    long tongslcp = db.CT_DHCD.Where(x => x.MADH == dhcdh.MADH).Sum(q => q.SLCP) ?? 0;
                    int sophieu = db.CT_DHCD.Where(x => x.MADH == dhcdh.MADH).Count();
                    List<BANGYKIENBQPHIEU> listBangYK = db.BANGYKIENBQPHIEUx.Where(x => x.MADH == dhcdh.MADH).ToList();
                    if (listBangYK != null && listBangYK.Count > 0)
                    {
                        foreach (BANGYKIENBQPHIEU v in listBangYK)
                        {
                            try
                            {
                                v.SL_PHATRA = sophieu;
                                v.SLCP_PHATRA = tongslcp;
                                TempData["Message"] = "Cập nhập thông tin Ý kiên biểu quyết thành công";
                                db.SaveChanges();
                            }
                            catch
                            {
                                TempData["Message"] = "Lỗi không thể cập nhật Số lượng cổ phiếu phát ra";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "Lỗi không thể cập nhật Số lượng cổ phiếu phát ra";
                }
            }
            else
            {
                TempData["Message"] = "Lỗi không thể cập nhật Số lượng cổ phiếu phát ra";
            }
            return RedirectToAction("Index");
        }


        // GET: /BIEUQUYETPHIEU/Details/5
        public ActionResult Details(int? id)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                 && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BANGYKIENBQPHIEU bangykien = db.BANGYKIENBQPHIEUx.Find(id);
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

        // GET: /BIEUQUYETPHIEU/Create
        public ActionResult Create()
        {
            if (HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty == "AdminQLDHCD"
                 && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                 && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                DHCD dhcdh = db.DHCDs.Where(x => x.ACTIVE == 1).OrderBy(q => q.thoiGian).First();
                BANGYKIENBQPHIEU bangyk = new BANGYKIENBQPHIEU();
                bangyk.MADH = dhcdh.MADH;
                bangyk.SLDONGY = 0;
                bangyk.NCPDONGY = 0;
                bangyk.NCPKHONGDONGY = 0;
                bangyk.SLKHONGDONGY = 0;
                bangyk.SL_PHATRA = 0;
                bangyk.SLCP_PHATRA = 0;
                bangyk.SL_THUVAO = 0;
                bangyk.SLCP_THUVAO = 0;
                bangyk.SL_HOPLE = 0;
                bangyk.SL_KHONG_HOPLE=0;
                bangyk.SLCP_HOPLE=0;
                bangyk.SLCP_KHONG_HOPLE=0;
                ViewBag.TenDH = dhcdh.TenDH;
                return View(bangyk);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }

        // POST: /BIEUQUYETPHIEU/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="MAYK,MADH,NOIDUNG,SLDONGY,NCPDONGY,SLKHONGDONGY,NCPKHONGDONGY,SL_PHATRA,SLCP_PHATRA,SL_THUVAO,SLCP_THUVAO,SL_HOPLE,SLCP_HOPLE,SL_KHONG_HOPLE,SLCP_KHONG_HOPLE")] BANGYKIENBQPHIEU bangykienbqphieu)
        {
            if (HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty == "AdminQLDHCD"
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                if (ModelState.IsValid)
                {
                    db.BANGYKIENBQPHIEUx.Add(bangykienbqphieu);
                    try
                    {
                        db.SaveChanges();
                        TempData["Message"] = "Tạo mới ý kiến biểu quyết thành công ";
                        return RedirectToAction("Index");
                    }
                    catch { }
                }

                ModelState.AddModelError("", "Dữ liệu nhập không đúng, vui lòng kiểm tra lại");
                DHCD dhcdh = db.DHCDs.Where(x => x.ACTIVE == 1).OrderBy(q => q.thoiGian).First();
                ViewBag.TenDH = dhcdh.TenDH;
                return View(bangykienbqphieu);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }


            
        }

        // GET: /BIEUQUYETPHIEU/Edit/5
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
                BANGYKIENBQPHIEU bangykienbqphieu = db.BANGYKIENBQPHIEUx.Find(id);
                if (bangykienbqphieu == null)
                {
                    return HttpNotFound();
                }

                DHCD dhcdh = db.DHCDs.Where(x => x.ACTIVE == 1).OrderBy(q => q.thoiGian).First();
                ViewBag.TenDH = dhcdh.TenDH;
                return View(bangykienbqphieu);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }

            
        }

        // POST: /BIEUQUYETPHIEU/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="MAYK,MADH,NOIDUNG,SLDONGY,NCPDONGY,SLKHONGDONGY,NCPKHONGDONGY,SL_PHATRA,SLCP_PHATRA,SL_THUVAO,SLCP_THUVAO,SL_HOPLE,SLCP_HOPLE,SL_KHONG_HOPLE,SLCP_KHONG_HOPLE")] BANGYKIENBQPHIEU bangykienbqphieu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bangykienbqphieu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MADH = new SelectList(db.DHCDs, "MADH", "TenDH", bangykienbqphieu.MADH);
            return View(bangykienbqphieu);
        }

        //// GET: /BIEUQUYETPHIEU/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BANGYKIENBQPHIEU bangykienbqphieu = db.BANGYKIENBQPHIEUx.Find(id);
        //    if (bangykienbqphieu == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(bangykienbqphieu);
        //}

        //// POST: /BIEUQUYETPHIEU/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    BANGYKIENBQPHIEU bangykienbqphieu = db.BANGYKIENBQPHIEUx.Find(id);
        //    db.BANGYKIENBQPHIEUx.Remove(bangykienbqphieu);
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
