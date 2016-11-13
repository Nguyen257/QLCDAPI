using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLDHCDAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string strMaDH = new DHCDController().GetCurrentMaDH();
            if(!string.IsNullOrWhiteSpace(strMaDH) && !string.Equals(strMaDH,"NULL"))
            {
                HttpContext.Session[Core.Define.SessionName.MaDH] = strMaDH;
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}