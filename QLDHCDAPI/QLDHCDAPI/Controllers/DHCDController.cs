using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLDHCDAPI.Models;
using System.Data.Entity;
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
        public ActionResult Index()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
                && (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin"))
            {
                QLDHCDEntities data = new QLDHCDEntities();
                List<DHCD> lst = new List<DHCD>();
                lst = (from lstdhcd in data.DHCDs select lstdhcd).ToList();
                return View(lst);
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
                DHCD f = new DHCD();
                
                return View(f);
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
                if(f.ACTIVE==1)
                {
                    List<DHCD> ListDHCD = (from l in db.DHCDs
                                           where l.ACTIVE == 1
                                           select l).ToList();
                    if(ListDHCD!=null && ListDHCD.Count>0)
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
                DHCD dhcd = (from v in data.DHCDs
                             where v.MADH == f.MADH
                             select v).First();
                dhcd.TenDH = f.TenDH;
                dhcd.nQKin = f.nQKin;
                dhcd.nQBefore = f.nQBefore;
                dhcd.nDeCuHDQT = f.nDeCuHDQT;
                dhcd.nDeCuBKS = f.nDeCuBKS;
                dhcd.nUngCuBKS = f.nUngCuBKS;
                dhcd.nUngCuHDQT = f.nUngCuHDQT;
                dhcd.thoiGian = f.thoiGian;
                dhcd.ACTIVE = f.ACTIVE;
                dhcd.TONGSOPHIEU = f.TONGSOPHIEU;

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
                QLDHCDEntities data = new QLDHCDEntities();
                try
                {
                    DHCD dhcd = (from v in data.DHCDs
                                 where v.MADH == madh
                                 select v).First();
                    data.DHCDs.Remove(dhcd);
                    data.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View("Delete");
                }

                
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }
        }


    }
}