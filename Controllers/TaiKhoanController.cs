using LTW.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTW.Controllers
{
    public class TaiKhoanController : Controller
    {
        DBConnect db = new DBConnect();
        
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DangNhap()
        {
            
            if (Session["rolename"] != null)
            {
                string role = Session["rolename"].ToString();
                if (role == "Admin")
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                else
                    return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(string tenDangNhap, string matKhau)
        {
            TaiKhoanData tkData = new TaiKhoanData();
            VaiTroData vtData = new VaiTroData();

            var nguoidung = tkData.dsTaiKhoan
                .FirstOrDefault(u =>
                    (u.TenDangNhap == tenDangNhap || u.Email == tenDangNhap)
                    && u.MatKhau == matKhau);

            if (nguoidung != null)
            {
                var vaitro = vtData.lstvaitro.FirstOrDefault(v => v.VaiTroID == nguoidung.VaiTroID);

                Session["userid"] = nguoidung.TaiKhoanID;
                Session["username"] = nguoidung.TenDangNhap;
                Session["email"] = nguoidung.Email;
                Session["roleid"] = nguoidung.VaiTroID;
                Session["rolename"] = vaitro?.TenVaiTro ?? "User";
                Session["ten"] = nguoidung.HoTen;

                
                if (Session["rolename"].ToString() == "Admin")
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }    
                   
                else
                {
                    return RedirectToAction("Index", "Home");
                }    
                   
            }
            else
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View();
            }
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(string tenDangNhap, string matKhau, string hoTen, string email, string soDienThoai, string diaChi)
        {
            TaiKhoanData tk = new TaiKhoanData();

           
            var trungten = tk.dsTaiKhoan.FirstOrDefault(u => u.TenDangNhap == tenDangNhap);
            var trungemail = tk.dsTaiKhoan.FirstOrDefault(u => u.Email == email);

            if (trungten != null)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View();
            }

            if (trungemail != null)
            {
                ViewBag.Error = "Email đã được sử dụng!";
                return View();
            }

            TaiKhoan newuser = new TaiKhoan
            {
                TenDangNhap = tenDangNhap,
                MatKhau = matKhau,
                HoTen = hoTen,
                Email = email,
                SoDienThoai = soDienThoai,
                DiaChi = diaChi,
                VaiTroID = 2 
            };

            bool inserted = tk.TaoTaiKhoan(newuser);

            if (inserted)
            {
                TempData["Success"] = "Đăng ký thành công! Hãy đăng nhập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            else
            {
                ViewBag.Error = "Tên đăng nhập hoặc email đã tồn tại!";
                return View();
            }
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        public ActionResult ThongTin()
        {
           
            if (Session["userid"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            int userId = Convert.ToInt32(Session["userid"]);

            TaiKhoanData tkData = new TaiKhoanData();
            VaiTroData vtData = new VaiTroData();

           
            var user = tkData.dsTaiKhoan.FirstOrDefault(u => u.TaiKhoanID == userId);
            if (user == null)
            {
                return RedirectToAction("DangNhap");
            }

            
            var vaitro = vtData.lstvaitro.FirstOrDefault(v => v.VaiTroID == user.VaiTroID);
            ViewBag.VaiTro = vaitro?.TenVaiTro ?? "Người dùng";

            return View(user);
        }
        [HttpGet]
        public ActionResult SuaThongTin()
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            int userId = Convert.ToInt32(Session["userid"]);
            TaiKhoanData tkData = new TaiKhoanData();
            var user = tkData.dsTaiKhoan.FirstOrDefault(u => u.TaiKhoanID == userId);

            if (user == null)
            {
                return RedirectToAction("DangNhap");
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult SuaThongTin(TaiKhoan model)
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = DBConnect.GetConnection())
            {
                string sql = @"UPDATE TaiKhoan 
                       SET HoTen = @HoTen,
                           Email = @Email,
                           SoDienThoai = @SoDienThoai,
                           DiaChi = @DiaChi
                       WHERE TaiKhoanID = @TaiKhoanID";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@HoTen", model.HoTen ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", model.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SoDienThoai", model.SoDienThoai ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChi", model.DiaChi ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TaiKhoanID", userId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("ThongTin");
        }

    }
}