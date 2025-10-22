using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTW.Models;

namespace LTW.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["rolename"] == null || Session["rolename"].ToString() != "Admin")
            {
                filterContext.Result = new RedirectResult("/TaiKhoan/DangNhap");
            }
            base.OnActionExecuting(filterContext);
        }
        
        public ActionResult Index()
        {
            var ngayHienTai = DateTime.Now;
            SanPhamData sp = new SanPhamData();
            var lstsp = sp.dsSanPham;
            TaiKhoanData tk = new TaiKhoanData();
            var lsttk = tk.dsTaiKhoan;
            ViewBag.SoSanPham = lstsp.Count;
            ViewBag.SoDonHang = 45;
            ViewBag.SoNguoiDung = lsttk.Count;
            ViewBag.DoanhThu = 78500000;

            ViewBag.NgayDoanhThu = new[] { "01/10", "02/10", "03/10", "04/10", "05/10", "06/10", "07/10" };
            ViewBag.GiaTriDoanhThu = new[] { 2000000, 5000000, 3500000, 8000000, 6000000, 9000000, 7500000 };

            var sanPhamMoi = lstsp
    .Where(x => x.NgayThem >= ngayHienTai.AddDays(-7)) 
    .OrderByDescending(x => x.NgayThem)
    .ToList();
            ViewBag.SanPhamMoi = sanPhamMoi;
            return View();
        }
    }
}