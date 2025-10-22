using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace LTW.Models
{
	public class GioHangChiTiet
	{
        public int GioHangChiTietID { get; set; }
        public int GioHangID { get; set; }
        public int SanPhamID { get; set; }
        public int SoLuong { get; set; }
        public DateTime NgayTao { get; set; }
    }
    public class GioHangChiTietData
    {
        public List<GioHangChiTiet> dsGioHangChiTiet = new List<GioHangChiTiet>();

        public void GetAll()
        {
            using (SqlConnection con = DBConnect.GetConnection())
            {
                string sql = "SELECT * FROM GioHangChiTiet";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    GioHangChiTiet ct = new GioHangChiTiet
                    {
                        GioHangChiTietID = Convert.ToInt32(dr["GioHangChiTietID"]),
                        GioHangID = Convert.ToInt32(dr["GioHangID"]),
                        SanPhamID = Convert.ToInt32(dr["SanPhamID"]),
                        SoLuong = Convert.ToInt32(dr["SoLuong"]),
                        NgayTao = dr["NgayTao"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["NgayTao"])
                    };
                    dsGioHangChiTiet.Add(ct);
                }
            }
        }

        public GioHangChiTietData()
        {
            GetAll();
        }
    }
}