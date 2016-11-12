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

namespace QLDHCDAPI.Controllers
{
    public class BangBQYKienController : Controller
    {
        //
        // GET: /BangBQYKien/
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            List<BANGYKIEN> lst = new List<BANGYKIEN>();
            lst = (from l in data.BANGYKIENs
                   where l.DHCD.ACTIVE == 1
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
                lst = lst.Where(s => s.NOIDUNG.Contains(searchString)).ToList();
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lst.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            BANGYKIEN cd = new BANGYKIEN();
            QLDHCDEntities data = new QLDHCDEntities();
            PopulateDHDropDownList();
            return View(cd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MADH,NOIDUNG,SLDONGY,SLKHONGDONGY,SLYKKHAC")] BANGYKIEN f)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    QLDHCDEntities data = new QLDHCDEntities();
                    data.BANGYKIENs.Add(f);
                    data.SaveChanges();
                    ViewBag.check = 1;
                    ViewBag.checkString = "Thêm thành công ";
                }
            }
            catch
            {
                ViewBag.checkString = "Thêm thất bại";
                ViewBag.check = 0;

            }
            PopulateDHDropDownList();
            return View(f);
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QLDHCDEntities data = new QLDHCDEntities();
            BANGYKIEN bangykien = data.BANGYKIENs.Find(id);
            if (bangykien == null)
            {
                return HttpNotFound();
            }
            return View(bangykien);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QLDHCDEntities data = new QLDHCDEntities();
            BANGYKIEN bangykien = data.BANGYKIENs.Find(id);
            if (bangykien == null)
            {
                return HttpNotFound();
            }
            PopulateDHDropDownList();
            return View(bangykien);
        }

        // POST: /aaa/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MAYK,MADH,NOIDUNG")] BANGYKIEN bangykien)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    QLDHCDEntities data = new QLDHCDEntities();
                    if (String.IsNullOrEmpty(bangykien.MADH)) bangykien.MADH = getMaDHActive();
                    data.Entry(bangykien).State = EntityState.Modified;
                    data.SaveChanges();
                    ViewBag.check = 1;
                    ViewBag.checkString = "Chỉnh sửa thành công !!!";
                    PopulateDHDropDownList();
                    return View();
                }
            }
            catch
            {
                ViewBag.check = 0;
                ViewBag.checkString = "Chỉnh sửa thất bại !!!";
            }
            PopulateDHDropDownList();
            return View(bangykien);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QLDHCDEntities data = new QLDHCDEntities();
            BANGYKIEN bangykien = data.BANGYKIENs.Find(id);
            if (bangykien == null)
            {
                return HttpNotFound();
            }
            return View(bangykien);
        }

        // POST: /aaa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            BANGYKIEN bangykien = data.BANGYKIENs.Find(id);
            data.BANGYKIENs.Remove(bangykien);
            data.SaveChanges();
            return RedirectToAction("Index");
        }





        public string getMaDHActive()
        {
            QLDHCDEntities data = new QLDHCDEntities();
            string dhQuery = (from d in data.DHCDs
                          where d.ACTIVE == 1
                          select d.MADH).First();
            return dhQuery;
        }
        public void PopulateDHDropDownList(object selectedDH = null)
        {
            QLDHCDEntities data = new QLDHCDEntities();
            var dhQuery = from d in data.DHCDs
                          where d.ACTIVE == 1
                          select d;
            ViewBag.MADH = new SelectList(dhQuery, "MADH", "TenDH", selectedDH);
        }
	}
}