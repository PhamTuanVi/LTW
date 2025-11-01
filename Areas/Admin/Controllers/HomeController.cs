using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        public ActionResult NguoiDung()
        {
            var tkData = new TaiKhoanData();

            var lstnguoidung = tkData.dsTaiKhoan.Where(u => u.VaiTroID == 2).ToList();
            return View(lstnguoidung);
        }
        [HttpPost]

        public ActionResult XoaNguoiDung(int id)
        {
            using (SqlConnection con = DBConnect.GetConnection())
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // 🔹 Xóa chi tiết đơn hàng
                    string sqlCTDH = @"
                        DELETE FROM ChiTietDonHang
                        WHERE DonHangID IN (SELECT DonHangID FROM DonHang WHERE TaiKhoanID = @TaiKhoanID)";
                    SqlCommand cmdCTDH = new SqlCommand(sqlCTDH, con, tran);
                    cmdCTDH.Parameters.AddWithValue("@TaiKhoanID", id);
                    cmdCTDH.ExecuteNonQuery();

                    // 🔹 Xóa đơn hàng
                    string sqlDH = "DELETE FROM DonHang WHERE TaiKhoanID = @TaiKhoanID";
                    SqlCommand cmdDH = new SqlCommand(sqlDH, con, tran);
                    cmdDH.Parameters.AddWithValue("@TaiKhoanID", id);
                    cmdDH.ExecuteNonQuery();

                    // 🔹 Xóa chi tiết giỏ hàng
                    string sqlGHCT = @"
                        DELETE FROM GioHangChiTiet
                        WHERE GioHangID IN (SELECT GioHangID FROM GioHang WHERE TaiKhoanID = @TaiKhoanID)";
                    SqlCommand cmdGHCT = new SqlCommand(sqlGHCT, con, tran);
                    cmdGHCT.Parameters.AddWithValue("@TaiKhoanID", id);
                    cmdGHCT.ExecuteNonQuery();

                    // 🔹 Xóa giỏ hàng
                    string sqlGH = "DELETE FROM GioHang WHERE TaiKhoanID = @TaiKhoanID";
                    SqlCommand cmdGH = new SqlCommand(sqlGH, con, tran);
                    cmdGH.Parameters.AddWithValue("@TaiKhoanID", id);
                    cmdGH.ExecuteNonQuery();

                    // 🔹 Xóa đánh giá
                    string sqlDG = "DELETE FROM DanhGia WHERE TaiKhoanID = @TaiKhoanID";
                    SqlCommand cmdDG = new SqlCommand(sqlDG, con, tran);
                    cmdDG.Parameters.AddWithValue("@TaiKhoanID", id);
                    cmdDG.ExecuteNonQuery();

                    // 🔹 Cuối cùng là xóa tài khoản
                    string sqlTK = "DELETE FROM TaiKhoan WHERE TaiKhoanID = @TaiKhoanID";
                    SqlCommand cmdTK = new SqlCommand(sqlTK, con, tran);
                    cmdTK.Parameters.AddWithValue("@TaiKhoanID", id);
                    int rows = cmdTK.ExecuteNonQuery();

                    tran.Commit();

                    if (rows > 0)
                        TempData["Message"] = "✅ Xóa tài khoản thành công!";
                    else
                        TempData["Error"] = "⚠️ Không tìm thấy tài khoản để xóa!";
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    TempData["Error"] = "❌ Lỗi khi xóa tài khoản: " + ex.Message;
                }
            }

            return RedirectToAction("NguoiDung");
        }
    }
}