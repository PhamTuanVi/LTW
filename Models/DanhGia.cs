using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace LTW.Models
{
	public class DanhGia
	{ 
        public int DanhGiaID { get; set; }
        public string NoiDung { get; set; }
        public int SoSao { get; set; }
        public int TaiKhoanID { get; set; }
        public int SanPhamID { get; set; }
        public string TraLoiAdmin { get; set; }
    }
    public class DanhGiaData
    {
        public List<DanhGia> dsDanhGia = new List<DanhGia>();

        public void GetAll()
        {
            using (SqlConnection con = DBConnect.GetConnection())
            {
                string sql = @"SELECT * FROM DanhGia";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    DanhGia dg = new DanhGia
                    {
                        DanhGiaID = Convert.ToInt32(dr["DanhGiaID"]),
                        NoiDung = dr["NoiDung"].ToString(),
                        SoSao = Convert.ToInt32(dr["SoSao"]),
                        TaiKhoanID = Convert.ToInt32(dr["TaiKhoanID"]),
                        SanPhamID = Convert.ToInt32(dr["SanPhamID"]),
                        TraLoiAdmin = dr["TraLoiAdmin"] == DBNull.Value ? null : dr["TraLoiAdmin"].ToString()
                    };
                    dsDanhGia.Add(dg);
                }
            }
        }

        public DanhGiaData()
        {
            GetAll();
        }
    }
}