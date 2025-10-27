using LTW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTW.Controllers
{
    public class SanPhamController : Controller
    {
        DBConnect db = new DBConnect();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ChiTiet(int id)
        {
            var sp = new SanPhamData();
            var lstsp = sp.dsSanPham.ToList();
            var ctsp = lstsp.FirstOrDefault(s => s.SanPhamID == id);
            return View(ctsp);
        }
        public PartialViewResult CauHinhPartial(int id)
        {
            ChiTietCauHinhData chData = new ChiTietCauHinhData();
            var ch = chData.GetBySanPhamID(id);

            return PartialView("CauHinhPartial", ch);
        }
    }
}