using LTW.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace LTW.Controllers
{
    public class DatHangController : Controller
    {
        // GET: DatHang/Index
        public ActionResult Index()
        {
            int taiKhoanId = Convert.ToInt32(Session["userid"]);
            
            if (taiKhoanId == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt hàng!";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Lấy giỏ hàng
            var giohangct = new GioHangChiTietData();
            var dsgiohangchitiet = giohangct.dsGioHangChiTiet.ToList();
            var sp = new SanPhamData();
            var lstsp = sp.dsSanPham.ToList();
            var giohang = new GioHangData();
            var dsgiohang = giohang.dsGioHang.ToList();
            
            var giohangcanhan = dsgiohang.FirstOrDefault(gh => gh.TaiKhoanID == taiKhoanId);
            
            List<GioHangChiTiet> giohangchitietcanhan = new List<GioHangChiTiet>();
            if (giohangcanhan != null)
            {
                giohangchitietcanhan = dsgiohangchitiet.Where(ct => ct.GioHangID == giohangcanhan.GioHangID).ToList();
            }

            if (!giohangchitietcanhan.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index", "GioHang");
            }
            foreach (var ct in giohangchitietcanhan)
            {
                var sanpham = lstsp.FirstOrDefault(s => s.SanPhamID == ct.SanPhamID);
                if (sanpham != null)
                {
                    if (ct.SoLuong > sanpham.SoLuong) // SoLuong là số lượng tồn kho
                    {
                        TempData["ErrorMessage"] = $"Sản phẩm {sanpham.TenSanPham} chỉ còn {sanpham.SoLuong} trong kho. Vui lòng điều chỉnh số lượng!";
                        return RedirectToAction("Index", "GioHang"); // Quay lại giỏ hàng
                    }
                }
            }

            ViewBag.ChiTiets = giohangchitietcanhan;
            ViewBag.SanPhams = lstsp;
            
            // ⭐ Lấy thông tin từ TaiKhoan làm giá trị mặc định
            var tkData = new TaiKhoanData();
            var user = tkData.dsTaiKhoan.FirstOrDefault(tk => tk.TaiKhoanID == taiKhoanId);
            
            ViewBag.HoTen = user?.HoTen ?? Session["ten"]?.ToString() ?? "";
            ViewBag.SoDienThoai = user?.SoDienThoai ?? "";
            ViewBag.DiaChi = user?.DiaChi ?? "";

            return View();
        }

        // POST: DatHang/XacNhanDatHang
        [HttpPost]
        public ActionResult XacNhanDatHang(string soDienThoai, string diaChi, string ghiChu, string phuongThucThanhToan)
        {
            int taiKhoanId = Convert.ToInt32(Session["userid"]);

            if (taiKhoanId == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập!";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Validation
            if (string.IsNullOrWhiteSpace(soDienThoai) || string.IsNullOrWhiteSpace(diaChi))
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin!";
                return RedirectToAction("Index");
            }

            // Lấy giỏ hàng
            var giohangct = new GioHangChiTietData();
            var dsgiohangchitiet = giohangct.dsGioHangChiTiet.ToList();
            var sp = new SanPhamData();
            var lstsp = sp.dsSanPham.ToList();
            var giohang = new GioHangData();
            var dsgiohang = giohang.dsGioHang.ToList();
            
            var giohangcanhan = dsgiohang.FirstOrDefault(gh => gh.TaiKhoanID == taiKhoanId);

            if (giohangcanhan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy giỏ hàng!";
                return RedirectToAction("Index", "GioHang");
            }

            var giohangchitietcanhan = dsgiohangchitiet.Where(ct => ct.GioHangID == giohangcanhan.GioHangID).ToList();

            if (!giohangchitietcanhan.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "GioHang");
            }

            // Tính tổng tiền
            decimal tongTien = 0;
            foreach (var ct in giohangchitietcanhan)
            {
                var sanpham = lstsp.FirstOrDefault(s => s.SanPhamID == ct.SanPhamID);
                if (sanpham != null)
                {
                    tongTien += sanpham.Gia * ct.SoLuong;
                }
            }

            try
            {
                using (var con = DBConnect.GetConnection())
                {
                    con.Open();

                    // ⭐ INSERT vào DonHang (KHÔNG có HoTen)
                    string sqlDonHang = @"
                        INSERT INTO DonHang 
                        (TaiKhoanID, NgayDat, TrangThaiID, TongTien, 
                         SoDienThoai, DiaChi, GhiChu, PhuongThucThanhToan) 
                        VALUES 
                        (@TaiKhoanID, GETDATE(), @TrangThaiID, @TongTien,
                         @SoDienThoai, @DiaChi, @GhiChu, @PhuongThucThanhToan); 
                        SELECT SCOPE_IDENTITY();";

                    int donHangId;
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sqlDonHang, con))
                    {
                        cmd.Parameters.AddWithValue("@TaiKhoanID", taiKhoanId);
                        cmd.Parameters.AddWithValue("@TrangThaiID", 1); // 1 = Chờ xử lý
                        cmd.Parameters.AddWithValue("@TongTien", tongTien);
                        cmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                        cmd.Parameters.AddWithValue("@DiaChi", diaChi);
                        cmd.Parameters.AddWithValue("@GhiChu", ghiChu ?? "");
                        cmd.Parameters.AddWithValue("@PhuongThucThanhToan", phuongThucThanhToan);

                        donHangId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Thêm chi tiết đơn hàng
                    foreach (var ct in giohangchitietcanhan)
                    {
                        var sanpham = lstsp.FirstOrDefault(s => s.SanPhamID == ct.SanPhamID);
                        if (sanpham != null)
                        {
                            string sqlChiTiet = @"
                                INSERT INTO ChiTietDonHang 
                                (DonHangID, SanPhamID, SoLuong, Gia) 
                                VALUES 
                                (@DonHangID, @SanPhamID, @SoLuong, @Gia)";

                            using (var cmd = new System.Data.SqlClient.SqlCommand(sqlChiTiet, con))
                            {
                                cmd.Parameters.AddWithValue("@DonHangID", donHangId);
                                cmd.Parameters.AddWithValue("@SanPhamID", ct.SanPhamID);
                                cmd.Parameters.AddWithValue("@SoLuong", ct.SoLuong);
                                cmd.Parameters.AddWithValue("@Gia", sanpham.Gia);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    // Xóa giỏ hàng
                    string sqlXoaGioHang = @"DELETE FROM GioHangChiTiet WHERE GioHangID = @GioHangID";
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sqlXoaGioHang, con))
                    {
                        cmd.Parameters.AddWithValue("@GioHangID", giohangcanhan.GioHangID);
                        cmd.ExecuteNonQuery();
                    }

                    TempData["SuccessMessage"] = "Đặt hàng thành công! Mã đơn hàng: " + donHangId;
                    return RedirectToAction("ThanhCong", new { id = donHangId });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đặt hàng thất bại: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: DatHang/ThanhCong
        public ActionResult ThanhCong(int id)
        {
            ViewBag.DonHangID = id;
            return View();
        }
        // ⭐ GET: DatHang/DonHangDaDat - Xem danh sách đơn hàng
        public ActionResult DonHangDaDat()
        {
            // Kiểm tra đăng nhập
            if (Session["userid"] == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem đơn hàng!";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            int taiKhoanId = Convert.ToInt32(Session["userid"]);

            // Load danh sách đơn hàng
            var donHangData = new DonHangData();
            var dsDonHang = donHangData.dsDonHang
                .Where(dh => dh.TaiKhoanID == taiKhoanId)
                .OrderByDescending(dh => dh.NgayDat)
                .ToList();

            return View(dsDonHang);
        }

        // ⭐ GET: DatHang/ChiTietDonHang/5 - Xem chi tiết 1 đơn hàng
        public ActionResult ChiTietDonHang(int id)
        {
            // Kiểm tra đăng nhập
            if (Session["userid"] == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập!";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            int taiKhoanId = Convert.ToInt32(Session["userid"]);

            // Load đơn hàng
            var donHangData = new DonHangData();
            var donHang = donHangData.dsDonHang
                .FirstOrDefault(dh => dh.DonHangID == id && dh.TaiKhoanID == taiKhoanId);

            if (donHang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng hoặc bạn không có quyền xem!";
                return RedirectToAction("DonHangDaDat");
            }

            // Load chi tiết đơn hàng
            var chiTietData = new ChiTietDonHangData();
            var dsChiTiet = chiTietData.dsChiTiet
                .Where(ct => ct.DonHangID == id)
                .ToList();

            // Load sản phẩm
            var spData = new SanPhamData();
            var dsSanPham = spData.dsSanPham.ToList();

            ViewBag.DonHang = donHang;
            ViewBag.ChiTiets = dsChiTiet;
            ViewBag.SanPhams = dsSanPham;

            return View();
        }

        // ⭐ POST: DatHang/HuyDonHang/5 - Hủy đơn hàng
        [HttpPost]
        public ActionResult HuyDonHang(int id)
        {
            if (Session["userid"] == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập!";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            int taiKhoanId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (var con = DBConnect.GetConnection())
                {
                    con.Open();

                    // Kiểm tra quyền và trạng thái
                    string checkSql = @"
                        SELECT TrangThaiID, TaiKhoanID 
                        FROM DonHang 
                        WHERE DonHangID = @DonHangID";

                    using (var cmd = new System.Data.SqlClient.SqlCommand(checkSql, con))
                    {
                        cmd.Parameters.AddWithValue("@DonHangID", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
                                return RedirectToAction("DonHangDaDat");
                            }

                            int trangThaiID = Convert.ToInt32(reader["TrangThaiID"]);
                            int ownerID = Convert.ToInt32(reader["TaiKhoanID"]);

                            // Kiểm tra quyền
                            if (ownerID != taiKhoanId)
                            {
                                TempData["ErrorMessage"] = "Bạn không có quyền hủy đơn hàng này!";
                                return RedirectToAction("DonHangDaDat");
                            }

                            // Chỉ cho phép hủy đơn "Chờ xử lý" (TrangThaiID = 1)
                            if (trangThaiID != 1)
                            {
                                TempData["ErrorMessage"] = "Chỉ có thể hủy đơn hàng đang chờ xử lý!";
                                return RedirectToAction("ChiTietDonHang", new { id = id });
                            }
                        }
                    }

                    // Cập nhật trạng thái thành "Đã hủy" (TrangThaiID = 4)
                    string updateSql = @"
                        UPDATE DonHang 
                        SET TrangThaiID = 4 
                        WHERE DonHangID = @DonHangID";

                    using (var cmd = new System.Data.SqlClient.SqlCommand(updateSql, con))
                    {
                        cmd.Parameters.AddWithValue("@DonHangID", id);
                        cmd.ExecuteNonQuery();
                    }

                    TempData["SuccessMessage"] = "Đã hủy đơn hàng thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hủy đơn hàng thất bại: " + ex.Message;
            }

            return RedirectToAction("DonHangDaDat");
        }
        [HttpPost]
        public ActionResult ThemDanhGia(int SanPhamID, int DonHangID, int SoSao, string NoiDung)
        {
            int taiKhoanId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (var con = DBConnect.GetConnection())
                {
                    con.Open();

                    string sql = @"
                INSERT INTO DanhGia (SanPhamID, DonHangID, SoSao, NoiDung, TaiKhoanID)
                VALUES (@SanPhamID, @DonHangID, @SoSao, @NoiDung, @TaiKhoanID)";

                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@SanPhamID", SanPhamID);
                        cmd.Parameters.AddWithValue("@DonHangID", DonHangID);
                        cmd.Parameters.AddWithValue("@SoSao", SoSao);
                        cmd.Parameters.AddWithValue("@NoiDung", NoiDung ?? "");
                        cmd.Parameters.AddWithValue("@TaiKhoanID", taiKhoanId);
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Cảm ơn bạn đã đánh giá!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đánh giá thất bại: " + ex.Message;
            }

            return RedirectToAction("DonHangDaDat");
        }

    }
}
