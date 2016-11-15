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
    public class UYQUYENController : Controller
    {
        private QLDHCDEntities db = new QLDHCDEntities();
        DAO Dao = new DAO();
        CultureInfo culture = CultureInfo.CurrentCulture;

        public UYQUYENController()
        {
            culture = new CultureInfo(1033);
        }

        #region Method
        private bool CheckUserNameNgChuyen(string matd,string userName)
        {
            bool DataReturn = false;
            try
            {
                if (!string.IsNullOrEmpty(matd) && !string.IsNullOrEmpty(userName))
                {
                    CT_DHCD ct_dhcd = db.CT_DHCD.Where(x => x.MATD == matd).First();
                    USERCD user = db.USERCDs.Where(x => x.USERNAME == userName).First();

                    if (ct_dhcd.MACD == user.MACD) return true;
                }
            }
            catch
            {
                return false;
            }
            return DataReturn;
        }
        #endregion

        // GET: /UYQUYEN/
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            ViewBag.Alert = TempData["Message"] + string.Empty;
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(x => x.thoiGian).ToList().First();

                List<UYQUYEN> lst = new List<UYQUYEN>();
                lst = (from l in db.UYQUYENs
                       where l.MADH == dhcd.MADH
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
                    lst = lst.Where(s => s.CT_DHCD.CODONG.HoTen.Contains(searchString) || s.CT_DHCD1.CODONG.HoTen.Contains(searchString)).ToList();
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

        // GET: /UYQUYEN/Details/5
        public ActionResult Details(int? id)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UYQUYEN uyquyen = db.UYQUYENs.Find(id);
                if (uyquyen == null)
                {
                    return HttpNotFound();
                }
                return View(uyquyen);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }

        // GET: /UYQUYEN/Create
        public ActionResult Create()
        {
            if ((HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty == "AdminQLDHCD")
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(x => x.thoiGian).ToList().First();

                UYQUYEN uq = new UYQUYEN();
                uq.MADH = dhcd.MADH;
                uq.THOIGIAN = DateTime.Now;
                ViewBag.TenDH = dhcd.TenDH;
                ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM + string.Empty;

                return View(uq);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }



        }

        // POST: /UYQUYEN/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MAUQ,MADH,MANGCHUYEN,UYQUYENTYPE,MANGNHAN,SLUQ,THOIGIAN")] UYQUYEN uyquyen, string HoTenNHAN, string CMND, string NgayCap, string NoiCap, string DiaChi, string QuocTich)
        {
            try
            {
                if ((HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty == "AdminQLDHCD")
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
                {
                    if (ModelState.IsValid)
                    {
                        if (uyquyen.SLUQ.HasValue)
                        {
                            int slSauCung = db.CT_DHCD.Where(x => x.MATD == uyquyen.MANGCHUYEN).ToList().First().SLCPSAUCUNG ?? 0;
                            if (!uyquyen.SLUQ.HasValue || uyquyen.SLUQ.Value > slSauCung)
                            {
                                DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(x => x.thoiGian).ToList().First();

                                ViewBag.TenDH = dhcd.TenDH;
                                ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM + string.Empty;

                                ModelState.AddModelError("", "Số lượng ủy quyền lớn hơn số lượng có thể ủy quyền");
                                return View(uyquyen);
                            }
                            DateTime? myDate = null;
                            DateTime tempDate = new DateTime();
                            int? cmnd = null; int tempCMND = 0;
                            if (DateTime.TryParse(NgayCap,out  tempDate))
                            {
                                myDate = tempDate;
                            }
                            if (!string.IsNullOrWhiteSpace(CMND))
                            {
                                if (int.TryParse(CMND, out tempCMND))
                                {
                                    cmnd = tempCMND;
                                }
                            }
                            ObjectParameter myCheck = new ObjectParameter("CHECKSUCCESS", 0);
                            db.UYQUYEN_INSERT(uyquyen.MANGCHUYEN, uyquyen.MANGNHAN, uyquyen.UYQUYENTYPE, uyquyen.SLUQ, HoTenNHAN, cmnd, myDate, NoiCap, DiaChi, QuocTich, myCheck);
                            if (myCheck.Value.ToString() =="1")
                            {
                                TempData["Message"] = "Đã ủy quyền " + uyquyen.SLUQ ?? "" + " thành công";
                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(x => x.thoiGian).ToList().First();

                                ViewBag.TenDH = dhcd.TenDH;
                                ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM + string.Empty;

                                ModelState.AddModelError("", "Ủy quyền không thành công");
                                return View(uyquyen);
                            }
                            
                        }

                    }
                    else
                    {
                        DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(x => x.thoiGian).ToList().First();

                        ViewBag.TenDH = dhcd.TenDH;
                        ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM + string.Empty;

                        ModelState.AddModelError("", "Đã xảy ra lỗi");
                        return View(uyquyen);
                    }

                }
                else
                {
                    DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(x => x.thoiGian).ToList().First();

                    ViewBag.TenDH = dhcd.TenDH;
                    ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM + string.Empty;

                    ModelState.AddModelError("", "Bạn không có quyền hạn này");
                    return View(uyquyen);
                }
            }
            catch(Exception ex)
            {
                return new HttpStatusCodeResult(400, "Loi Uy QUyen");
            }
            return new HttpStatusCodeResult(400, "Loi Uy QUyen");


        }

        // GET: /UYQUYEN/Create
        public ActionResult UyQuyenUser()
        {
            if ((!string.IsNullOrEmpty(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty))
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "User"))
            {
                try
                {
                    string UserName = HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty;
                    DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(x => x.thoiGian).ToList().First();
                    USERCD CurrentUser = db.USERCDs.Where(x => x.USERNAME == UserName).First();

                    CT_DHCD NguoiChuyen = (from l in db.CT_DHCD
                                           where l.MACD == CurrentUser.MACD && l.MADH == dhcd.MADH
                                           select l).First();

                    UYQUYEN uq = new UYQUYEN();
                    uq.MANGCHUYEN = NguoiChuyen.MATD;
                    uq.MADH = dhcd.MADH;
                    uq.THOIGIAN = DateTime.Now;
                    ViewBag.TenNguoiChuyen = NguoiChuyen.CODONG.HoTen;
                    ViewBag.TenDH = dhcd.TenDH;
                    string macodinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM + string.Empty;
                    ViewBag.MaCoDinh = macodinh;
                    string[] ArrayTemp = NguoiChuyen.MATD.Split(new string[] { macodinh }, StringSplitOptions.RemoveEmptyEntries);
                    string CDchuyen = "";
                    for(int i=0;i<ArrayTemp.Length;i++)
                    {
                        CDchuyen += ArrayTemp[i] + string.Empty;
                    }
                    ViewBag.CDChuyen = CDchuyen;

                    return View(uq);
                }
                catch(Exception ex)
                {
                    TempData["Message"] = "Đã có lỗi xảy ra" + ex.Message;
                    return RedirectToAction("Index");
                }
                
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }

        // POST: /UYQUYEN/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UyQuyenUser([Bind(Include = "MAUQ,MADH,MANGCHUYEN,UYQUYENTYPE,MANGNHAN,SLUQ,THOIGIAN")] UYQUYEN uyquyen, string HoTenNHAN, string CMND, string NgayCap, string NoiCap, string DiaChi, string QuocTich)
        {
            try
            {
                if ((!string.IsNullOrEmpty(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty))
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "User"))
                {
                    if (CheckUserNameNgChuyen(uyquyen.MANGCHUYEN, HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty))
                    {
                        if (ModelState.IsValid)
                        {
                            if (uyquyen.SLUQ.HasValue && uyquyen.SLUQ.Value > 0)
                            {
                                int slSauCung = db.CT_DHCD.Where(x => x.MATD == uyquyen.MANGCHUYEN).ToList().First().SLCPSAUCUNG ?? 0;
                                if (uyquyen.SLUQ.Value > slSauCung)
                                {
                                    DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(x => x.thoiGian).ToList().First();

                                    ViewBag.TenDH = dhcd.TenDH;
                                    ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM + string.Empty;

                                    ModelState.AddModelError("", "Số lượng ủy quyền lớn hơn số lượng có thể ủy quyền");
                                    return View(uyquyen);
                                }
                                DateTime? myDate = null;
                                DateTime tempDate = new DateTime();
                                int? cmnd = null; int tempCMND = 0;
                                if (DateTime.TryParse(NgayCap, out  tempDate))
                                {
                                    myDate = tempDate;
                                }
                                if (!string.IsNullOrWhiteSpace(CMND))
                                {
                                    if (int.TryParse(CMND, out tempCMND))
                                    {
                                        cmnd = tempCMND;
                                    }
                                }
                                ObjectParameter myCheck = new ObjectParameter("CHECKSUCCESS", 0);
                                string Username = HttpContext.Session[Core.Define.SessionName.UserName]+string.Empty;
                                db.UYQUYEN_INSERT_BYUSERNAME(Username, uyquyen.MANGNHAN, uyquyen.UYQUYENTYPE, uyquyen.SLUQ, HoTenNHAN, cmnd, myDate, NoiCap, DiaChi, QuocTich, myCheck);
                                if (myCheck.Value.ToString() == "1")
                                {
                                    TempData["Message"] = "Đã ủy quyền " + uyquyen.SLUQ ?? "" + " thành công";
                                    db.SaveChanges();
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(x => x.thoiGian).ToList().First();

                                    ViewBag.TenDH = dhcd.TenDH;
                                    ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM + string.Empty;

                                    ModelState.AddModelError("", "Ủy quyền không thành công");
                                    return View(uyquyen);
                                }

                            }

                        }
                        else
                        {
                            DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(x => x.thoiGian).ToList().First();

                            ViewBag.TenDH = dhcd.TenDH;
                            ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM + string.Empty;

                            ModelState.AddModelError("", "Đã xảy ra lỗi");
                            return View(uyquyen);
                        }

                    }
                }
                else
                {
                    DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(x => x.thoiGian).ToList().First();

                    ViewBag.TenDH = dhcd.TenDH;
                    ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM + string.Empty;

                    ModelState.AddModelError("", "Bạn không có quyền hạn này");
                    return View(uyquyen);
                }
            }
            catch(Exception ex)
            {
                return new HttpStatusCodeResult(400, "Loi Uy QUyen");
            }
            return new HttpStatusCodeResult(400, "Loi Uy QUyen");


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
