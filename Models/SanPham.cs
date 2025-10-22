using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace LTW.Models
{
	public class SanPham
	{
        public int SanPhamID { get; set; }
        public string TenSanPham { get; set; }
        public string ThuongHieu { get; set; }
        public string MoTa { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public string HinhAnh1 { get; set; }
        public DateTime NgayThem { get; set; }
        public int DanhMucID { get; set; }
        public string TenDanhMuc { get; set; }
    }
    public class SanPhamData
    {
        public List<SanPham> dsSanPham = new List<SanPham>();

        public void GetAll()
        {
            using (SqlConnection con = DBConnect.GetConnection())
            {
                string sql = @"
                    SELECT s.*, d.TenDanhMuc
                    FROM SanPham s
                    LEFT JOIN DanhMuc d ON s.DanhMucID = d.DanhMucID";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    SanPham sp = new SanPham
                    {
                        SanPhamID = Convert.ToInt32(dr["SanPhamID"]),
                        TenSanPham = dr["TenSanPham"].ToString(),
                        ThuongHieu = dr["ThuongHieu"].ToString(),
                        MoTa = dr["MoTa"].ToString(),
                        SoLuong = Convert.ToInt32(dr["SoLuong"]),
                        Gia = Convert.ToDecimal(dr["Gia"]),
                        HinhAnh1 = dr["HinhAnh1"].ToString(),
                        NgayThem = dr["NgayThem"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["NgayThem"]),
                        DanhMucID = dr["DanhMucID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DanhMucID"]),
                        TenDanhMuc = dr["TenDanhMuc"] == DBNull.Value ? "Không rõ" : dr["TenDanhMuc"].ToString()
                    };
                    dsSanPham.Add(sp);
                }
            }
        }
       

        public SanPhamData()
        {
            GetAll();
            
        }
    }
}