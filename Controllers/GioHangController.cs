using LTW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Text;

namespace LTW.Controllers
{
    public class GioHangController : Controller
    {
        
        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult MiniCart()
        {
            int taiKhoanId = Convert.ToInt32(Session["userid"]);
            var giohangct = new GioHangChiTietData();
            var dsgiohangchitiet = giohangct.dsGioHangChiTiet.ToList();
            var sp = new SanPhamData();
            var lstsp = sp.dsSanPham.ToList();
            var giohang = new GioHangData();
            var dsgiohang = giohang.dsGioHang.ToList();
            var giohangcanhan = dsgiohang.FirstOrDefault(gh => gh.TaiKhoanID == taiKhoanId);
            List<GioHangChiTiet> giohangchitietcanhan = new List<GioHangChiTiet>();
            if(giohangcanhan!=null)
            {
                giohangchitietcanhan = dsgiohangchitiet.Where(ct => ct.GioHangID == giohangcanhan.GioHangID).ToList();
            }
            ViewBag.ChiTiets = giohangchitietcanhan;
            ViewBag.SanPhams = lstsp;
            return PartialView("MiniCart");
        }
    }
}
//Session["userid"] = nguoidung.TaiKhoanID;
//Session["username"] = nguoidung.TenDangNhap;
//Session["email"] = nguoidung.Email;
//Session["roleid"] = nguoidung.VaiTroID;
//Session["rolename"] = vaitro?.TenVaiTro ?? "User";
//Session["ten"] = nguoidung.HoTen;