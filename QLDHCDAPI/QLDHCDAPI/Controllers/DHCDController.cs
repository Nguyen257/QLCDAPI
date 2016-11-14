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


namespace QLDHCDAPI.Controllers
{
    public class DHCDController : Controller
    {
        QLDHCDEntities db = new QLDHCDEntities();

        public string GetCurrentMaDH()
        {
            string DataReturn = "NULL";
            try
            {
                List<DHCD> ListDHCD = (from l in db.DHCDs
                                       where l.ACTIVE == 1
                                       select l).ToList();
                if (ListDHCD != null && ListDHCD.Count > 0)
                {
                    DataReturn = ListDHCD.First().MADH;
                }
            }
            catch
            {
                return "NULL";
            }
            return DataReturn;
        }
        //
        // GET: /DHCD/
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {

            ViewBag.Alert = TempData["Message"] + string.Empty;
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                QLDHCDEntities data = new QLDHCDEntities();
                List<DHCD> lst = new List<DHCD>();
                lst = (from lstdhcd in data.DHCDs where lstdhcd.TRANGTHAI.Value == false select lstdhcd).ToList();
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
                    lst = lst.Where(s => s.TenDH.Contains(searchString)).ToList();
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
        public ActionResult Create()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                DHCD dhcd = new DHCD();

                return View(dhcd);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }

        public ActionResult AddUngVienHDQT()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                try
                {
                    DHCD_ThanhVien ThanhVien = new DHCD_ThanhVien();
                    ThanhVien.DHCD = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(o => o.thoiGian).ToList().First();
                    ThanhVien.lstHDQT = new THANHVIENHDQT();
                    ThanhVien.lstBKS = new THANHVIENBK();
                    ThanhVien.THAMDU = new CT_DHCD();
                    ViewBag.MaCoDinh = "HDC" + ThanhVien.DHCD.YEARDHCD + ThanhVien.DHCD.STTDHTRONGNAM;

                    return View(ThanhVien);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUngVienHDQT(string myDataForm)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                try
                {
                    string[] ArrayValue = myDataForm.Split(new string[] { "##$$##" }, StringSplitOptions.RemoveEmptyEntries);

                    if (ArrayValue.Length > 0 && !string.IsNullOrWhiteSpace(ArrayValue[0]) && ArrayValue[0].Contains("HDC"))
                    {
                        string tempMaTD = ArrayValue[0] + string.Empty;
                        CT_DHCD tempTV = db.CT_DHCD.Where(x => x.MATD == tempMaTD).ToList().First();
                        if (tempTV != null)
                        {
                            string myMDH = db.CT_DHCD.Where(x => x.MATD == tempTV.MATD).First().MADH;
                            List<THANHVIENHDQT> listErrTV = db.THANHVIENHDQTs.Where(x => x.CT_DHCD.MADH == myMDH).ToList();
                            if (listErrTV != null && listErrTV.Count > 0)
                            {
                                foreach (THANHVIENHDQT v in listErrTV)
                                {
                                    db.THANHVIENHDQTs.Remove(v);
                                }
                                db.SaveChanges();
                            }
                        }
                    }

                    for (int i = 0; i < ArrayValue.Length - 1; i = i + 2)
                    {
                        string matd = ArrayValue[i];
                        string type = ArrayValue[i + 1];

                        List<CT_DHCD> listTD = db.CT_DHCD.Where(x => x.MATD == matd).ToList();
                        if (listTD != null && listTD.Count > 0)
                        {
                            THANHVIENHDQT tvhdqt = new THANHVIENHDQT();
                            tvhdqt.MATD = matd;
                            switch (type)
                            {
                                case "decu": tvhdqt.LADECU = true; break;
                                case "ungcu": tvhdqt.LAUNGCU = true; break;
                                default: tvhdqt.LATHAYTHE = true; break;
                            }
                            tvhdqt.SLPHIEUBAU = 0;
                            tvhdqt.LACHUTICH = false;
                            tvhdqt.LASUCCESS = false;

                            db.THANHVIENHDQTs.Add(tvhdqt);
                        }
                        else
                        {
                            DHCD_ThanhVien ThanhVien = new DHCD_ThanhVien();
                            ThanhVien.DHCD = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(o => o.thoiGian).ToList().First();
                            ThanhVien.lstHDQT = new THANHVIENHDQT();
                            ThanhVien.lstBKS = new THANHVIENBK();
                            ThanhVien.THAMDU = new CT_DHCD();
                            ViewBag.MaCoDinh = "HDC" + ThanhVien.DHCD.YEARDHCD + ThanhVien.DHCD.STTDHTRONGNAM;
                            ModelState.AddModelError("error", "cổ đông chưa đăng ký tham dự đại hội");
                            ViewBag.AlertErr = "Cổ đông chưa đăng ký tham dự đại hội";
                            return View(ThanhVien);
                        }




                    }
                    db.SaveChanges();
                    TempData["Message"] = "Thêm ứng viên vào HĐQT thành công";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(400, " Loi " + ex.Message);
                }
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }

        public ActionResult AddUngVienBKS()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                try
                {
                    DHCD_ThanhVien ThanhVien = new DHCD_ThanhVien();
                    ThanhVien.DHCD = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(o => o.thoiGian).ToList().First();
                    ThanhVien.lstHDQT = new THANHVIENHDQT();
                    ThanhVien.lstBKS = new THANHVIENBK();
                    ThanhVien.THAMDU = new CT_DHCD();
                    ViewBag.MaCoDinh = "HDC" + ThanhVien.DHCD.YEARDHCD + ThanhVien.DHCD.STTDHTRONGNAM;

                    return View(ThanhVien);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUngVienBKS(string myDataForm)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                try
                {
                    string[] ArrayValue = myDataForm.Split(new string[] { "##$$##" }, StringSplitOptions.RemoveEmptyEntries);
                    bool checkLaBoSung = false;
                    if (ArrayValue.Length > 0 && !string.IsNullOrWhiteSpace(ArrayValue[0]) && ArrayValue[0].Contains("HDC"))
                    {
                        string tempMaTD = ArrayValue[0] + string.Empty;
                        CT_DHCD tempTV = db.CT_DHCD.Where(x => x.MATD == tempMaTD).ToList().First();
                        if (tempTV != null)
                        {
                            string mdh = db.CT_DHCD.Where(x => x.MATD == tempTV.MATD).First().MADH;
                            checkLaBoSung = tempTV.DHCD.LABAUBOSUNG;
                            List<THANHVIENBK> listErrTV = db.THANHVIENBKS.Where(x => x.CT_DHCD.MADH == myMDH).ToList();
                            if (listErrTV != null && listErrTV.Count > 0)
                            {
                                foreach (THANHVIENBK v in listErrTV)
                                {
                                    db.THANHVIENBKS.Remove(v);
                                }
                                db.SaveChanges();
                            }
                        }

                    }
                    
                    if(checkLaBoSung)
                    {
                        try
                        {
                            List<COMMITDHCD> listCommit = db.COMMITDHCDs.OrderByDescending(x => x.DATECOMMIT.Value).ToList();
                            if (listCommit != null && listCommit.Count > 0)
                            {
                                COMMITDHCD lastCommit = listCommit.First();
                                string mdh = lastCommit.MADH;
                                List<THANHVIENBK> listOldHDQT = db.THANHVIENBKS.Where(x => x.CT_DHCD.MADH == mdh).ToList();
                                if(listOldHDQT!=null && listOldHDQT.Count>0)
                                {
                                    foreach(THANHVIENBK v in listOldHDQT)
                                    {
                                        THANHVIENBK OldTV = new THANHVIENBK();
                                        OldTV.MATD = 'HDC'+v.CT_DHCD.DHCD.YEARDHCD + 
                                    }
                                    db.SaveChanges();
                                }
                            }
                        }
                        catch
                        {

                        }
                         
                    }

                    for (int i = 0; i < ArrayValue.Length - 1; i = i + 2)
                    {
                        string matd = ArrayValue[i];
                        string type = ArrayValue[i + 1];

                        List<CT_DHCD> listTD = db.CT_DHCD.Where(x => x.MATD == matd).ToList();
                        if (listTD != null && listTD.Count > 0)
                        {
                            THANHVIENBK tvbks = new THANHVIENBK();
                            tvbks.MATD = matd;
                            switch (type)
                            {
                                case "decu": tvbks.LADECU = true; break;
                                case "ungcu": tvbks.LAUNGCU = true; break;
                                default: tvbks.LATHAYTHE = true; break;
                            }
                            tvbks.SLPHIEUBAU = 0;
                            tvbks.LACHUTICH = false;
                            tvbks.LASUCCESS = false;

                            db.THANHVIENBKS.Add(tvbks);
                            
                        }
                        else
                        {
                            DHCD_ThanhVien ThanhVien = new DHCD_ThanhVien();
                            ThanhVien.DHCD = db.DHCDs.Where(x => x.ACTIVE == 1).OrderByDescending(o => o.thoiGian).ToList().First();
                            ThanhVien.lstHDQT = new THANHVIENHDQT();
                            ThanhVien.lstBKS = new THANHVIENBK();
                            ThanhVien.THAMDU = new CT_DHCD();
                            ViewBag.MaCoDinh = "HDC" + ThanhVien.DHCD.YEARDHCD + ThanhVien.DHCD.STTDHTRONGNAM;
                            ModelState.AddModelError("error", "cổ đông chưa đăng ký tham dự đại hội");
                            @ViewBag.AlertErr = "Cổ đông chưa đăng ký tham dự đại hội";
                            return View(ThanhVien);
                        }

                    }
                    db.SaveChanges();
                    TempData["Message"] = "Thêm  ứng viên vào HĐQT thành công";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(400, " Loi " + ex.Message );
                }
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }

        public ActionResult InsertDHCD(DHCD f)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                QLDHCDEntities data = new QLDHCDEntities();
                if (f.ACTIVE == 1)
                {
                    List<DHCD> ListDHCD = (from l in db.DHCDs
                                           where l.ACTIVE == 1
                                           select l).ToList();
                    if (ListDHCD != null && ListDHCD.Count > 0)
                    {
                        DHCD dhcd = ListDHCD.First();
                        dhcd.ACTIVE = 0;
                        db.Entry(dhcd).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                data.DHCDs.Add(f);
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }

        public ActionResult EditDHCD(string madh)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                QLDHCDEntities data = new QLDHCDEntities();
                DHCD dhcd = (from v in data.DHCDs
                             where v.MADH == madh
                             select v).First();
                return View("EditDHCD", dhcd);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }

        }
        public ActionResult UpdateDHCD(DHCD f)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                QLDHCDEntities data = new QLDHCDEntities();
                if (f.ACTIVE == 1)
                {
                    List<DHCD> ListDHCD = (from l in db.DHCDs
                                           where l.ACTIVE == 1
                                           select l).ToList();
                    if (ListDHCD != null && ListDHCD.Count > 0)
                    {
                        DHCD curretnActivedhcd = ListDHCD.First();
                        curretnActivedhcd.ACTIVE = 0;

                        db.Entry(curretnActivedhcd).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (f.TRANGTHAI.HasValue)
                {
                    f.TRANGTHAI = true;
                }
                else
                    f.TRANGTHAI = false;

                data.Entry(f).State = EntityState.Modified;
                //DHCD dhcd = (from v in data.DHCDs
                //             where v.MADH == f.MADH
                //             select v).First();
                //dhcd.TenDH = f.TenDH;
                //dhcd.nQKin = f.nQKin;
                //dhcd.nQBefore = f.nQBefore;
                //dhcd.nDeCuHDQT = f.nDeCuHDQT;
                //dhcd.nDeCuBKS = f.nDeCuBKS;
                //dhcd.nUngCuBKS = f.nUngCuBKS;
                //dhcd.nUngCuHDQT = f.nUngCuHDQT;
                //dhcd.thoiGian = f.thoiGian;
                //dhcd.ACTIVE = f.ACTIVE;
                //dhcd.TONGSOPHIEU = f.TONGSOPHIEU;

                data.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }

        }

        public ActionResult DetailDHCD(string madh)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                QLDHCDEntities data = new QLDHCDEntities();
                DHCD dhcd = (from v in data.DHCDs
                             where v.MADH == madh
                             select v).First();
                ViewBag.madh = dhcd.MADH;
                return View(dhcd);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }

        [HttpGet]
        public ActionResult Delete(string madh)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                DHCD dhcd = (from v in db.DHCDs
                             where v.MADH == madh
                             select v).First();
                return View(dhcd);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }

        [HttpPost]
        public ActionResult DeleteDHCD(string madh)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                try
                {
                    DHCD dhcd = (from v in db.DHCDs
                                 where v.MADH == madh
                                 select v).ToList().First();
                    dhcd.TRANGTHAI = true;
                    db.Entry(dhcd).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(400);
                }


            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }


    }
}