using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace LTW.Models
{
    public class TrangThaiDonHangData
    {
        public List<TrangThaiDonHang> dsTrangThai = new List<TrangThaiDonHang>();

        public void GetAll()
        {
            using (SqlConnection con = DBConnect.GetConnection())
            {
                string sql = "SELECT * FROM TrangThaiDonHang ORDER BY TrangThaiID";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    TrangThaiDonHang tt = new TrangThaiDonHang
                    {
                        TrangThaiID = Convert.ToInt32(dr["TrangThaiID"]),
                        TenTrangThai = dr["TenTrangThai"].ToString()
                    };
                    dsTrangThai.Add(tt);
                }
            }
        }

        public TrangThaiDonHangData()
        {
            GetAll();
        }
    }
}
