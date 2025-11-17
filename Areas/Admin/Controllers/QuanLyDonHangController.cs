using LTW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace LTW.Areas.Admin.Controllers
{
    public class QuanLyDonHangController : Controller
    {
        // ⭐ Kiểm tra quyền admin cho tất cả action
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["rolename"] == null || Session["rolename"].ToString() != "Admin")
            {
                filterContext.Result = new RedirectResult("/TaiKhoan/DangNhap");
            }
            base.OnActionExecuting(filterContext);
        }

        // GET: Admin/QuanLyDonHang/Index
        public ActionResult Index(int? trangThai = null, string search = "")
        {
            // Load danh sách đơn hàng
            var donHangData = new DonHangData();
            var dsDonHang = donHangData.dsDonHang.OrderByDescending(dh => dh.NgayDat).ToList();

            // Lọc theo trạng thái
            if (trangThai.HasValue && trangThai.Value > 0)
            {
                dsDonHang = dsDonHang.Where(dh => dh.TrangThaiID == trangThai.Value).ToList();
            }

            // Tìm kiếm
            if (!string.IsNullOrWhiteSpace(search))
            {
                dsDonHang = dsDonHang.Where(dh =>
                    dh.DonHangID.ToString().Contains(search) ||
                    dh.HoTen.ToLower().Contains(search.ToLower()) ||
                    dh.SoDienThoai.Contains(search)
                ).ToList();
            }

            // Load trạng thái
            var trangThaiData = new TrangThaiDonHangData();
            ViewBag.DanhSachTrangThai = trangThaiData.dsTrangThai;
            ViewBag.TrangThaiSelected = trangThai;
            ViewBag.SearchKeyword = search;

            // Thống kê
            ViewBag.TongDonHang = donHangData.dsDonHang.Count;
            ViewBag.DonChoXuLy = donHangData.dsDonHang.Count(dh => dh.TrangThaiID == 1);
            ViewBag.DonDangGiao = donHangData.dsDonHang.Count(dh => dh.TrangThaiID == 2);
            ViewBag.DonHoanThanh = donHangData.dsDonHang.Count(dh => dh.TrangThaiID == 3);
            ViewBag.DonDaHuy = donHangData.dsDonHang.Count(dh => dh.TrangThaiID == 4);

            return View(dsDonHang);
        }

        // GET: Admin/QuanLyDonHang/ChiTiet/5
        public ActionResult ChiTiet(int id)
        {
            var donHangData = new DonHangData();
            var donHang = donHangData.dsDonHang.FirstOrDefault(dh => dh.DonHangID == id);

            if (donHang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("Index");
            }

            var chiTietData = new ChiTietDonHangData();
            var dsChiTiet = chiTietData.dsChiTiet.Where(ct => ct.DonHangID == id).ToList();

            var spData = new SanPhamData();
            var dsSanPham = spData.dsSanPham.ToList();

            var trangThaiData = new TrangThaiDonHangData();
            ViewBag.DanhSachTrangThai = trangThaiData.dsTrangThai;

            ViewBag.DonHang = donHang;
            ViewBag.ChiTiets = dsChiTiet;
            ViewBag.SanPhams = dsSanPham;

            return View();
        }

        // POST: Admin/QuanLyDonHang/XacNhan/5
        [HttpPost]
        public ActionResult XacNhan(int id)
        {
            try
            {
                using (var con = DBConnect.GetConnection())
                {
                    con.Open();

                    // Kiểm tra trạng thái
                    string checkSql = "SELECT TrangThaiID FROM DonHang WHERE DonHangID = @DonHangID";
                    using (var cmd = new System.Data.SqlClient.SqlCommand(checkSql, con))
                    {
                        cmd.Parameters.AddWithValue("@DonHangID", id);
                        var result = cmd.ExecuteScalar();
                        if (result == null)
                        {
                            TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
                            return RedirectToAction("Index");
                        }

                        int trangThai = Convert.ToInt32(result);
                        if (trangThai != 1)
                        {
                            TempData["ErrorMessage"] = "Chỉ có thể xác nhận đơn hàng đang chờ xử lý!";
                            return RedirectToAction("ChiTiet", new { id });
                        }
                    }

                    // Cập nhật trạng thái
                    string updateSql = "UPDATE DonHang SET TrangThaiID = 2 WHERE DonHangID = @DonHangID";
                    using (var cmd = new System.Data.SqlClient.SqlCommand(updateSql, con))
                    {
                        cmd.Parameters.AddWithValue("@DonHangID", id);
                        cmd.ExecuteNonQuery();
                    }

                    TempData["SuccessMessage"] = "✅ Đã xác nhận đơn hàng #" + id;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ Xác nhận thất bại: " + ex.Message;
            }

            return RedirectToAction("ChiTiet", new { id });
        }

        // POST: Admin/QuanLyDonHang/HoanThanh/5
        [HttpPost]
        public ActionResult HoanThanh(int id)
        {
            try
            {
                using (var con = DBConnect.GetConnection())
                {
                    string sql = @"UPDATE DonHang SET TrangThaiID = 3 
                                   WHERE DonHangID = @DonHangID AND TrangThaiID = 2";
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@DonHangID", id);
                        con.Open();
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                            TempData["SuccessMessage"] = "✅ Đã hoàn thành đơn hàng #" + id;
                        else
                            TempData["ErrorMessage"] = "⚠️ Chỉ có thể hoàn thành đơn hàng đang giao!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ Lỗi: " + ex.Message;
            }

            return RedirectToAction("ChiTiet", new { id });
        }

        // POST: Admin/QuanLyDonHang/Huy/5
        [HttpPost]
        public ActionResult Huy(int id, string lyDo)
        {
            if (string.IsNullOrWhiteSpace(lyDo))
            {
                TempData["ErrorMessage"] = "⚠️ Vui lòng nhập lý do hủy!";
                return RedirectToAction("ChiTiet", new { id });
            }

            try
            {
                using (var con = DBConnect.GetConnection())
                {
                    string sql = @"UPDATE DonHang 
                                   SET TrangThaiID = 4,
                                       GhiChu = ISNULL(GhiChu, '') + CHAR(13) + CHAR(10) + '[Admin hủy] ' + @LyDo
                                   WHERE DonHangID = @DonHangID";
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@DonHangID", id);
                        cmd.Parameters.AddWithValue("@LyDo", lyDo);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    TempData["SuccessMessage"] = "✅ Đã hủy đơn hàng #" + id;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ Lỗi: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // GET: Admin/QuanLyDonHang/ThongKe
        public ActionResult ThongKe()
        {
            var donHangData = new DonHangData();
            var dsDonHang = donHangData.dsDonHang;

            ViewBag.TongDonHang = dsDonHang.Count;
            ViewBag.TongDoanhThu = dsDonHang.Where(dh => dh.TrangThaiID == 3).Sum(dh => dh.TongTien);
            ViewBag.DonChoXuLy = dsDonHang.Count(dh => dh.TrangThaiID == 1);
            ViewBag.DonDangGiao = dsDonHang.Count(dh => dh.TrangThaiID == 2);
            ViewBag.DonHoanThanh = dsDonHang.Count(dh => dh.TrangThaiID == 3);
            ViewBag.DonDaHuy = dsDonHang.Count(dh => dh.TrangThaiID == 4);

            var homNay = DateTime.Today;
            ViewBag.DonHangHomNay = dsDonHang.Count(dh => dh.NgayDat.Date == homNay);
            ViewBag.DoanhThuHomNay = dsDonHang
                .Where(dh => dh.NgayDat.Date == homNay && dh.TrangThaiID == 3)
                .Sum(dh => dh.TongTien);

            // Top sản phẩm bán chạy
            var chiTietData = new ChiTietDonHangData();
            var spData = new SanPhamData();

            var topSanPham = chiTietData.dsChiTiet
                .GroupBy(ct => ct.SanPhamID)
                .Select(g => new
                {
                    SanPhamID = g.Key,
                    SoLuongBan = g.Sum(ct => ct.SoLuong),
                    DoanhThu = g.Sum(ct => ct.Gia * ct.SoLuong)
                })
                .OrderByDescending(x => x.SoLuongBan)
                .Take(5)
                .ToList();

            ViewBag.TopSanPham = topSanPham;
            ViewBag.DanhSachSanPham = spData.dsSanPham;

            return View();
        }
    }
}
