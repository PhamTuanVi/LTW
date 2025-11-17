using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTW.Models
{
    public class DonHang
    {
        public int DonHangID { get; set; }
        public int TaiKhoanID { get; set; }
        public DateTime NgayDat { get; set; }
        public int TrangThaiID { get; set; }
        public decimal TongTien { get; set; }
        public bool DaTruKho { get; set; }

        //  Thông tin lưu trong DonHang
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string GhiChu { get; set; }
        public string PhuongThucThanhToan { get; set; }

        // Thông tin JOIN từ TaiKhoan (không lưu trong DB)
        public string HoTen { get; set; }
        public string Email { get; set; }

        //  Thông tin JOIN từ TrangThaiDonHang
        public string TenTrangThai { get; set; }

    }
}