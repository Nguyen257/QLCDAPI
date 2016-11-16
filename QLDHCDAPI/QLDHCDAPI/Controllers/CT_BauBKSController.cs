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
    public class CT_BauBKSController : Controller
    {
        private QLDHCDEntities db = new QLDHCDEntities();
        DAO Dao = new DAO();
        CultureInfo culture = CultureInfo.CurrentCulture;
        public CT_BauBKSController()
        {
            culture = new CultureInfo(1033);
        }
        // GET: /CT_BauBKS/
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            ViewBag.Alert = TempData["Message"] + string.Empty;
            ViewBag.ListAlert = TempData["listMess"];
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {

                List<CT_BAUBKS> lst = new List<CT_BAUBKS>();
                DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(q => q.thoiGian).First();

                lst = (from l in db.CT_BAUBKS where l.THANHVIENBK.CT_DHCD.MADH == dhcd.MADH select l).ToList();
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
                string htdkOldTV = "BKS đại hội trước";
                DHCD dhcd = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(q => q.thoiGian).First();
                List<THANHVIENBK> listHDQT = (from l in db.THANHVIENBKS
                                                where l.CT_DHCD.MADH == dhcd.MADH && l.HINHTHUCBAU != htdkOldTV
                                                select l).ToList();
                string userName = HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty;
                USERCD CurrentUser = db.USERCDs.Where(x => x.USERNAME == userName).First();


                CT_BAUBKS CurrentCT = new CT_BAUBKS();
                CurrentCT.SLPHIEUBAU = 0;
                CurrentCT.THOIGIANBAU = DateTime.Now;
                CurrentCT.LAHOPLE = true;
                CurrentCT.HINHTHUCBAU = "Trực tiếp";

                long SoPhieuThuVao = db.CT_BAUBKS.Where(x => x.CT_DHCD.DHCD.MADH == dhcd.MADH ).Sum(q => q.SLPHIEUBAU);
                long SoPhieuHopLe = db.CT_BAUBKS.Where(x => x.CT_DHCD.DHCD.MADH == dhcd.MADH  && x.LAHOPLE == true).Sum(q => q.SLPHIEUBAU);
                long SoPhieuKhongHople = SoPhieuThuVao - SoPhieuHopLe;
                ViewBag.SLHopLe = SoPhieuHopLe;
                ViewBag.SLKhongHopLe = SoPhieuKhongHople;
                ViewBag.SoPhieuThuVao = SoPhieuThuVao;
                ViewBag.SoPhieuPhatRa = dhcd.SLCPPHATRA_HDQT;
                ViewBag.TichSo = listHDQT.Count;
                ViewBag.MaCoDinh = "HDC" + dhcd.YEARDHCD + dhcd.STTDHTRONGNAM;
                ViewBag.YearDH = dhcd.YEARDHCD;

                ListModel<CT_BAUBKS> listReturn = new ListModel<CT_BAUBKS>();

                //List<KeyValuePair<THANHVIENHDQT, CT_BAUHDQT>> listCT = new List<KeyValuePair<THANHVIENHDQT, CT_BAUHDQT>>();
                foreach (THANHVIENBK v in listHDQT)
                {
                    CT_BAUBKS Row = new CT_BAUBKS();
                    Row.SLPHIEUBAU = 0;
                    Row.THOIGIANBAU = DateTime.Now;
                    Row.LAHOPLE = true;
                    Row.HINHTHUCBAU = "Trực tiếp";
                    Row.MABKS = v.MATD;
                    Row.THANHVIENBK = v;
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
        public ActionResult BauThanhVien(ListModel<CT_BAUBKS> listReturn)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
              && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
              && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                List<string> Mess = new List<string>();
                foreach (CT_BAUBKS v in listReturn.Items)
                {
                    THANHVIENHDQTController tvhdqtController = new THANHVIENHDQTController();
                    try
                    {
                        if (tvhdqtController.checkThanhVien(v.MABKS))
                        {
                            ObjectParameter myCheck = new ObjectParameter("CHECKSUCCESS", 0);
                            db.CT_BAUHDQT_INSERT(v.MABKS, v.MANGUOIBAU, v.SLPHIEUBAU, v.HINHTHUCBAU, v.LAHOPLE, myCheck);
                            if (myCheck.Value.ToString() == "1")
                            {
                                Mess.Add("Bầu thành viên " + v.MABKS + " thành công ");
                            }
                            else
                            {
                                Mess.Add("Bầu thành viên " + v.MABKS + "không thành công ");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Mess.Add("Bầu thành viên " + v.MABKS + " không thành công ");
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


        // GET: /CT_BauBKS/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CT_BAUBKS ct_baubks = db.CT_BAUBKS.Find(id);
            if (ct_baubks == null)
            {
                return HttpNotFound();
            }
            return View(ct_baubks);
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
