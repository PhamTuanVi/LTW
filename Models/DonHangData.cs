using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace LTW.Models
{
    public class DonHangData
    {
        public List<DonHang> dsDonHang = new List<DonHang>();

        public void GetAll()
        {
            using (SqlConnection con = DBConnect.GetConnection())
            {
                // ⭐ JOIN với TaiKhoan để lấy HoTen
                string sql = @"
                    SELECT 
                        dh.DonHangID, dh.TaiKhoanID, dh.NgayDat, dh.TrangThaiID, 
                        dh.TongTien, dh.DaTruKho,
                        dh.SoDienThoai, dh.DiaChi, dh.GhiChu, dh.PhuongThucThanhToan,
                        tk.HoTen, tk.Email,
                        tt.TenTrangThai
                    FROM DonHang dh
                    INNER JOIN TaiKhoan tk ON dh.TaiKhoanID = tk.TaiKhoanID
                    LEFT JOIN TrangThaiDonHang tt ON dh.TrangThaiID = tt.TrangThaiID
                    ORDER BY dh.NgayDat DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    DonHang dh = new DonHang
                    {
                        DonHangID = Convert.ToInt32(dr["DonHangID"]),
                        TaiKhoanID = Convert.ToInt32(dr["TaiKhoanID"]),
                        NgayDat = Convert.ToDateTime(dr["NgayDat"]),
                        TrangThaiID = Convert.ToInt32(dr["TrangThaiID"]),
                        TongTien = Convert.ToDecimal(dr["TongTien"]),
                        DaTruKho = dr["DaTruKho"] == DBNull.Value ? false : Convert.ToBoolean(dr["DaTruKho"]),

                        // Từ DonHang
                        SoDienThoai = dr["SoDienThoai"].ToString(),
                        DiaChi = dr["DiaChi"].ToString(),
                        GhiChu = dr["GhiChu"] == DBNull.Value ? "" : dr["GhiChu"].ToString(),
                        PhuongThucThanhToan = dr["PhuongThucThanhToan"].ToString(),

                        // ⭐ Từ TaiKhoan (JOIN)
                        HoTen = dr["HoTen"].ToString(),
                        Email = dr["Email"].ToString(),

                        // Từ TrangThaiDonHang
                        TenTrangThai = dr["TenTrangThai"] == DBNull.Value ? "" : dr["TenTrangThai"].ToString()
                    };
                    dsDonHang.Add(dh);
                }
            }
        }

        // Lấy đơn hàng theo ID
        public DonHang GetByID(int donHangID)
        {
            return dsDonHang.FirstOrDefault(dh => dh.DonHangID == donHangID);
        }

        // Lấy đơn hàng của 1 tài khoản
        public List<DonHang> GetByTaiKhoanID(int taiKhoanID)
        {
            return dsDonHang.Where(dh => dh.TaiKhoanID == taiKhoanID).ToList();
        }

        public DonHangData()
        {
            GetAll();
        }
    }
}
