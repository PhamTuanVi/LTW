using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace LTW.Models
{
	public class GioHang
	{
        public int GioHangID { get; set; }
        public int TaiKhoanID { get; set; }
    }
    public class GioHangData
    {
        public List<GioHang> dsGioHang = new List<GioHang>();

        public void GetAll()
        {
            using (SqlConnection con = DBConnect.GetConnection())
            {
                string sql = "SELECT * FROM GioHang";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    GioHang gh = new GioHang
                    {
                        GioHangID = Convert.ToInt32(dr["GioHangID"]),
                        TaiKhoanID = Convert.ToInt32(dr["TaiKhoanID"])
                    };
                    dsGioHang.Add(gh);
                }
            }
        }

        public GioHangData()
        {
            GetAll();
        }
    }
}