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
using System.Globalization;

namespace QLDHCDAPI.Controllers
{
    public class CoDongController : Controller
    {
        private QLDHCDEntities db = new QLDHCDEntities();
        CultureInfo culture = CultureInfo.CurrentCulture;

        public CoDongController()
        {
            culture = new CultureInfo(1033);
        }
        // GET: /CoDong/
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (!string.IsNullOrEmpty(HttpContext.Session[Core.Define.SessionName.Role] + string.Empty))
                {
                    ViewBag.Alert = TempData["Message"] + string.Empty;
                    QLDHCDEntities data = new QLDHCDEntities();
                    List<CODONG> lst = new List<CODONG>();
                    lst = (from l in data.CODONGs select l).ToList();

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
                        lst = lst.Where(s => s.HoTen.Contains(searchString)).ToList();
                    }
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return View(lst.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "User")
                    {
                        QLDHCDEntities data = new QLDHCDEntities();
                        USERCD User = null;
                        try
                        {
                            string currentUserName = HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty;
                            List<USERCD> lstUser = (from l in db.USERCDs
                                                    where l.USERNAME == currentUserName
                                                    select l).ToList();
                            if (lstUser != null && lstUser.Count > 0)
                                User = lstUser.First();
                        }
                        catch (Exception ex)
                        {
                            return new HttpStatusCodeResult(400, "Error Index CoDong  - " + ex.Message);
                        }

                        if (User != null)
                        {
                            List<CODONG> lst = (from l in data.CODONGs
                                                where l.MACD == User.MACD
                                                select l).ToList();
                            if (lst != null && lst.Count > 0)
                            {
                                return Details(lst.First().MACD);
                            }
                        }


                    }
                    return new HttpStatusCodeResult(400, "Error Index CoDong  ");
                }

            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - QLDHCD");
            }

        }

        // GET: /CoDong/Details/5
        public ActionResult Details(int? macd)
        {
            if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "User" || HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
            {

                if (macd == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CODONG codong = db.CODONGs.Find(macd);
                if (codong == null)
                {
                    return HttpNotFound();
                }
                return View("Details", codong);
            }
            else
            {
                return new HttpStatusCodeResult(401, "QLDHCD not allow you");
            }
        }

        // GET: /CoDong/Create
        public ActionResult Create()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                   && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
                {
                    return View();
                }
            }
            return new HttpStatusCodeResult(401);
        }

        // POST: /CoDong/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MACD,HoTen,CMND,NgayCap,NoiCap,DiaChi,QuocTich,ChucVu,Email,SDT,TrinhDoVanHoa,TrinhDoChuyenMon,ANHCD")] CODONG codong)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                   && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
                {
                    if (ModelState.IsValid)
                    {
                        db.CODONGs.Add(codong);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }

                    return View(codong);
                }
            }
            return new HttpStatusCodeResult(401);

        }

        // GET: /CoDong/Edit/5
        public ActionResult Edit(int? macd)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                      && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin" )
                {
                    
                    if (macd == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                    }
                    CODONG codong = db.CODONGs.Find(macd);
                    if (codong == null)
                    {
                        return HttpNotFound();
                    }
                    return View(codong);
                }
                else
                {
                    if(HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "User")
                    {
                        if (macd == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                        }
                        string userName = HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty;
                        USERCD User = db.USERCDs.Where(x => x.USERNAME == userName).First();
                        if (macd != User.MACD) return new HttpStatusCodeResult(401);
                        else
                        {
                            CODONG codong = db.CODONGs.Find(macd);
                            if (codong == null)
                            {
                                return HttpNotFound();
                            }
                            return View(codong);
                        }
                    }
                }
            }
            return new HttpStatusCodeResult(401);

        }

        // POST: /CoDong/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MACD,HoTen,CMND,NgayCap,NoiCap,DiaChi,QuocTich,ChucVu,Email,SDT,TrinhDoVanHoa,TrinhDoChuyenMon,ANHCD")] CODONG codong)
        {


            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                      && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
                {
                    if (ModelState.IsValid)
                    {
                        db.Entry(codong).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    return View(codong);
                }
                else
                {
                    if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "User")
                    {
                        string userName = HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty;
                        USERCD User = db.USERCDs.Where(x => x.USERNAME == userName).First();
                        if (codong.MACD != User.MACD) return new HttpStatusCodeResult(401);
                        else
                        {
                            if (ModelState.IsValid)
                            {
                                db.Entry(codong).State = EntityState.Modified;
                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }
                            return View(codong);
                        }
                    }

                }
            }
            return new HttpStatusCodeResult(401);

        }

        // GET: /CoDong/Delete/5
        public ActionResult Delete(int? macd)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                      && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
                {
                    if (macd == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    CODONG codong = db.CODONGs.Find(macd);
                    if (codong == null)
                    {
                        return HttpNotFound();
                    }
                    return View(codong);
                }
            }
            return new HttpStatusCodeResult(401);
            
        }

        // POST: /CoDong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int macd)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session[Core.Define.SessionName.UserName] + string.Empty)
                   && (HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes"))
            {
                if (HttpContext.Session[Core.Define.SessionName.Role] + string.Empty == "Admin")
                {
                    CODONG codong = db.CODONGs.Find(macd);
                    db.CODONGs.Remove(codong);
                    db.SaveChanges();
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
