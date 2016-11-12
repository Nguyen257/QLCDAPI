using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLDHCDAPI.Models;
using PagedList;
using PagedList.Mvc;
namespace QLDHCDAPI.Controllers
{
    public class CoDongController : Controller
    {
        QLDHCDEntities db = new QLDHCDEntities();
        //
        // GET: /CoDong/
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
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
            return View(lst.ToPagedList(pageNumber,pageSize));
        }

        public ActionResult Create()
        {
            CODONG cd = new CODONG();
            return View(cd);
        }

        public ActionResult InsertCD(CODONG f)
        {
            
            try
            {
                QLDHCDEntities data = new QLDHCDEntities();
                data.CODONGs.Add(f);
                data.SaveChanges();
                ViewBag.kq = "Đã thêm cổ đông thành công !!!";
            }
            catch
            {
                ViewBag.kq = "Thêm cổ đông thất bại !!!";
            }
            return View("InsertResult");
        }

        public ActionResult DetailCoDong(int macd)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            CODONG cd = (from v in data.CODONGs
                         where v.MACD == macd
                         select v).First();
            return View(cd);
        }
        public ActionResult EditCoDong(int macd)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            CODONG cd = (from v in data.CODONGs
                         where v.MACD == macd
                         select v).First();
            return View("EditCoDong", cd);

        }

        public ActionResult UpdateCoDong (CODONG f)
        {
            try
            {


                QLDHCDEntities data = new QLDHCDEntities();
                CODONG cd = (from v in data.CODONGs
                             where v.MACD == f.MACD
                             select v).First();

                cd.HoTen = f.HoTen;
                cd.CMND = f.CMND;
                cd.NgayCap = f.NgayCap;
                cd.NoiCap = f.NoiCap;
                cd.DiaChi = f.DiaChi;
                cd.QuocTich = f.QuocTich;
                cd.ChucVu = f.ChucVu;
                cd.Email = f.Email;
                cd.SDT = f.SDT;

                data.SaveChanges();
                ViewBag.kq = "Chỉnh sửa cổ đông thành công";
            }
            catch
            {
                ViewBag.kq = " Chỉnh sửa cổ đông thất bại ";
            }
            return View("UpdateResult");
        }

        public ActionResult DeleteCoDong(int macd)
        {
            try
            {
                QLDHCDEntities data = new QLDHCDEntities();
                CODONG cd = (from v in data.CODONGs
                             where v.MACD == macd
                             select v).First();
                data.CODONGs.Remove(cd);
                data.SaveChanges();

                ViewBag.kq = "Xóa cổ đông thành công ";
            }
            catch
            {
                ViewBag.kq = "Xóa cổ đông thất bại ";

            }
            return View("DeleteResult");
        }

        #region Method
        public string GetHoTen(string macd)
        {
            if (!string.IsNullOrWhiteSpace(macd))
            {
                try
                {
                    int MaCoDong = int.Parse(macd);
                    List<CODONG> ListCD = (from l in db.CODONGs
                                       where l.MACD == MaCoDong
                                       select l).ToList();
                    
                    if(ListCD.Count>0)
                    {
                        CODONG cd = ListCD.First();
                        return cd.HoTen;
                    }
                    return string.Empty;
                }
                catch { return string.Empty; }

                return string.Empty;
            }
            else
                return string.Empty;
        }

        #endregion
    }
}