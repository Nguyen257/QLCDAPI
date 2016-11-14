using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLDHCDAPI.Models;
using System.Security.Cryptography;
using System.Text;

namespace QLDHCDAPI.Controllers
{
    public class UserCoDongController : Controller
    {
        private QLDHCDEntities db = new QLDHCDEntities();
        private Core.DAO DAO = new Core.DAO();


        #region Method
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        private bool UserExists(string username)
        {
            bool DataReturn = false;
            try
            {
                List<USERCD> listUserCD = (from l in db.USERCDs
                                           where l.USERNAME == username
                                           select l).ToList();
                if (listUserCD.Count > 0)
                {
                    DataReturn = true;
                }
            }
            catch (Exception ex)
            {
                DAO.SaveException("CheckUserExists", ex.Message);
            }

            return DataReturn;
        }
        public byte[] getHashPass(string pass)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes;
            UTF8Encoding encoder = new UTF8Encoding();
            hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(pass));
            return hashedDataBytes;
        }
        public string GetRoleCurrentUser(string userName, string pass)
        {
            string DataReturn = string.Empty;
            try
            {
                byte[] PassUser = getHashPass(pass);
                List<USERCD> ListUserCD = (from l in db.USERCDs
                                           where l.USERNAME == userName && l.PASS == PassUser
                                           select l).ToList();
                if (ListUserCD != null && ListUserCD.Count > 0)
                {
                    if (userName == "AdminQLDHCD") return "Admin";
                    else return "User";
                }
            }
            catch (Exception ex)
            {
                DAO.SaveException("GetRoleCurrentUser - " + userName, ex.Message);
                return string.Empty;
            }
            return DataReturn;

        }
        #endregion

        // GET: /UserCoDong/
        public ActionResult Index()
        {
            ViewBag.Alert = TempData["Message"] + string.Empty;
            if (string.Equals(HttpContext.Session["UserName"] + string.Empty, "AdminQLDHCD") && !string.IsNullOrWhiteSpace(HttpContext.Session["isLogin"] + string.Empty))
            {
                var usercds = db.USERCDs.Include(u => u.CODONG);
                return View(usercds.ToList());
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - UserCoDong");
            }
        }

        public ActionResult Login()
        {
            if(HttpContext!=null && HttpContext.Session!=null && HttpContext.Session[Core.Define.SessionName.isLogin] + string.Empty == "Yes")
            {
                return RedirectToAction("Index", "Home");
            }
            string strMaDH = new DHCDController().GetCurrentMaDH();
            if (!string.IsNullOrWhiteSpace(strMaDH) && !string.Equals(strMaDH, "NULL"))
            {
                HttpContext.Session[Core.Define.SessionName.MaDH] = strMaDH;
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string USERNAME, string PASS)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes;
            UTF8Encoding encoder = new UTF8Encoding();
            hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(PASS));


            List<USERCD> ListUserCD = (from l in db.USERCDs
                                       where l.USERNAME == USERNAME && l.PASS == hashedDataBytes
                                       select l).ToList();
            USERCD UserCD = null;
            if (ListUserCD.Count > 0)
            {
                UserCD = ListUserCD.First();
                HttpContext.Session["isLogin"] = "Yes";
                HttpContext.Session["UserName"] = USERNAME;
                if (USERNAME == "AdminQLDHCD")
                    HttpContext.Session["Role"] = "Admin";
                else
                    HttpContext.Session["Role"] = "User";
                return RedirectToLocal("~/");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            if (string.Equals(HttpContext.Session["isLogin"] + string.Empty, "Yes"))
            {
                HttpContext.Session["isLogin"] = "No";
                HttpContext.Session["UserName"] = string.Empty;
                HttpContext.Session["Role"] = string.Empty;
            }
            return RedirectToLocal("~/");
        }


        public ActionResult Register()
        {
            if (string.Equals(HttpContext.Session["UserName"] + string.Empty, "AdminQLDHCD") && !string.IsNullOrWhiteSpace(HttpContext.Session["isLogin"] + string.Empty))
                return View();
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - UserCoDong");
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string USERNAME, string PASS, string ConfirmPass)
        {
            if (string.Equals(HttpContext.Session["UserName"] + string.Empty, "AdminQLDHCD") && !string.IsNullOrWhiteSpace(HttpContext.Session["isLogin"] + string.Empty))
            {
                if (!string.IsNullOrWhiteSpace(USERNAME) && !string.IsNullOrWhiteSpace(PASS) && !string.IsNullOrWhiteSpace(ConfirmPass))
                {
                    if (!UserExists(USERNAME))
                    {
                        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
                        byte[] hashedDataBytes;
                        UTF8Encoding encoder = new UTF8Encoding();
                        hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(PASS));

                        USERCD usercd = new USERCD();
                        usercd.USERNAME = USERNAME;
                        usercd.PASS = hashedDataBytes;

                        db.USERCDs.Add(usercd);
                        db.SaveChanges();
                        TempData["Message"] = "Chỉnh sửa tham dự đại hội thành công";
                        return RedirectToLocal("~/");
                    }
                    else
                    {
                        ViewBag.AlertText = "User đã tồn tại";
                        return View();
                    }
                }
                else
                {
                    ViewBag.AlertText = "Đã có lỗi xảy ra";
                    return View();
                }
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - UserCoDong");
            }

        }

        public ActionResult Manage()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session["UserName"] + string.Empty) && !string.IsNullOrWhiteSpace(HttpContext.Session["isLogin"] + string.Empty))
                return View();
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - UserCoDong");
            }
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(string USERNAME, string PASS, string ConfirmPass)
        {
            if (string.Equals(HttpContext.Session["UserName"] + string.Empty, USERNAME) && !string.IsNullOrWhiteSpace(HttpContext.Session["isLogin"] + string.Empty))
            {
                if (!string.IsNullOrWhiteSpace(USERNAME) && !string.IsNullOrWhiteSpace(PASS) && !string.IsNullOrWhiteSpace(ConfirmPass))
                {

                    List<USERCD> listUserCD = (from l in db.USERCDs
                                               where l.USERNAME == USERNAME
                                               select l).ToList();
                    if(listUserCD!=null && listUserCD.Count>0)
                    {
                        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
                        byte[] hashedDataBytes;
                        UTF8Encoding encoder = new UTF8Encoding();
                        hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(PASS));

                        USERCD usercd = listUserCD.First();
                        usercd.PASS = hashedDataBytes;

                        db.Entry(usercd).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToLocal("~/");
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    ViewBag.AlertText = "Đã có lỗi xảy ra";
                    return View();
                }
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - UserCoDong");
            }

        }

        // GET: /UserCoDong/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USERCD usercd = db.USERCDs.Find(id);
            if (usercd == null)
            {
                return HttpNotFound();
            }
            return View(usercd);
        }

        // GET: /UserCoDong/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (string.Equals(HttpContext.Session["UserName"] + string.Empty, "AdminQLDHCD") && !string.IsNullOrWhiteSpace(HttpContext.Session["isLogin"] + string.Empty))
            {
                USERCD usercd = db.USERCDs.Find(id);
                if (usercd == null)
                {
                    return HttpNotFound();
                }
                ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen", usercd.MACD);
                return View(usercd);
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - UserCoDong");
            }

            //return new HttpStatusCodeResult(404, "Error in cloud - UserCoDong");

        }

        // POST: /UserCoDong/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,USERNAME,PASS,MACD")] USERCD usercd)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usercd).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen", usercd.MACD);
            return View(usercd);
        }

        // GET: /UserCoDong/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (string.Equals(HttpContext.Session["UserName"] + string.Empty, "AdminQLDHCD") && !string.IsNullOrWhiteSpace(HttpContext.Session["isLogin"] + string.Empty))
            {
                USERCD usercd = db.USERCDs.Find(id);
                if (usercd == null)
                {
                    return HttpNotFound();
                }
                if (usercd.USERNAME == "AdminQLDHCD" || usercd.ID == 1)
                {
                    return new HttpStatusCodeResult(401, "Error in cloud - UserCoDong");
                }
                else
                {
                    ViewBag.MACD = new SelectList(db.CODONGs, "MACD", "HoTen", usercd.MACD);
                    return View(usercd);
                }
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - UserCoDong");
            }
        }

        // POST: /UserCoDong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (string.Equals(HttpContext.Session["UserName"] + string.Empty, "AdminQLDHCD") && !string.IsNullOrWhiteSpace(HttpContext.Session["isLogin"] + string.Empty))
            {
                USERCD usercd = db.USERCDs.Find(id);
                if (usercd.USERNAME == "AdminQLDHCD" || usercd.ID==1)
                {
                    return new HttpStatusCodeResult(401, "Error in cloud - UserCoDong");
                }
                else
                {
                    db.USERCDs.Remove(usercd);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return new HttpStatusCodeResult(401, "Error in cloud - UserCoDong");
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
