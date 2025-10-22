using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace LTW.Models
{
	public class ChiTietDonHang
	{
        public int ChiTietID { get; set; }
        public int DonHangID { get; set; }
        public int SanPhamID { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
    }
    public class ChiTietDonHangData
    {
        public List<ChiTietDonHang> dsChiTiet = new List<ChiTietDonHang>();

        public void GetAll()
        {
            using (SqlConnection con = DBConnect.GetConnection())
            {
                string sql = "SELECT * FROM ChiTietDonHang";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    ChiTietDonHang ct = new ChiTietDonHang
                    {
                        ChiTietID = Convert.ToInt32(dr["ChiTietID"]),
                        DonHangID = Convert.ToInt32(dr["DonHangID"]),
                        SanPhamID = Convert.ToInt32(dr["SanPhamID"]),
                        SoLuong = Convert.ToInt32(dr["SoLuong"]),
                        Gia = Convert.ToDecimal(dr["Gia"])
                    };
                    dsChiTiet.Add(ct);
                }
            }
        }

        public ChiTietDonHangData()
        {
            GetAll();
        }
    }
}