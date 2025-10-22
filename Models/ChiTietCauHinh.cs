using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace LTW.Models
{
	public class ChiTietCauHinh
	{
        public int CauHinhID { get; set; }
        public int SanPhamID { get; set; }
        public string ManHinh { get; set; }
        public string ChipXuLy { get; set; }
        public string RAM { get; set; }
        public string BoNhoTrong { get; set; }
        public string Pin { get; set; }
        public string HeDieuHanh { get; set; }
    }
    public class ChiTietCauHinhData
    {
        public List<ChiTietCauHinh> dsCauHinh = new List<ChiTietCauHinh>();

       
        public void GetAll()
        {
            using (SqlConnection con = DBConnect.GetConnection())
            {
                string sql = "SELECT * FROM ChiTietCauHinh";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    ChiTietCauHinh ch = new ChiTietCauHinh
                    {
                        CauHinhID = Convert.ToInt32(dr["CauHinhID"]),
                        SanPhamID = Convert.ToInt32(dr["SanPhamID"]),
                        ManHinh = dr["ManHinh"].ToString(),
                        ChipXuLy = dr["ChipXuLy"].ToString(),
                        RAM = dr["RAM"].ToString(),
                        BoNhoTrong = dr["BoNhoTrong"].ToString(),
                        Pin = dr["Pin"].ToString(),
                        HeDieuHanh = dr["HeDieuHanh"].ToString()
                    };
                    dsCauHinh.Add(ch);
                }
            }
        }

     
        public ChiTietCauHinh GetBySanPhamID(int sanPhamId)
        {
            using (SqlConnection con = DBConnect.GetConnection())
            {
                string sql = "SELECT * FROM ChiTietCauHinh WHERE SanPhamID = @SanPhamID";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@SanPhamID", sanPhamId);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ChiTietCauHinh
                        {
                            CauHinhID = Convert.ToInt32(reader["CauHinhID"]),
                            SanPhamID = Convert.ToInt32(reader["SanPhamID"]),
                            ManHinh = reader["ManHinh"].ToString(),
                            ChipXuLy = reader["ChipXuLy"].ToString(),
                            RAM = reader["RAM"].ToString(),
                            BoNhoTrong = reader["BoNhoTrong"].ToString(),
                            Pin = reader["Pin"].ToString(),
                            HeDieuHanh = reader["HeDieuHanh"].ToString()
                        };
                    }
                }
            }
            return null;
        }
        public bool Update(ChiTietCauHinh ch)
        {
            using (SqlConnection con = DBConnect.GetConnection())
            {
                string sql = @"UPDATE ChiTietCauHinh SET
                        ManHinh = @ManHinh,
                        ChipXuLy = @ChipXuLy,
                        RAM = @RAM,
                        BoNhoTrong = @BoNhoTrong,
                        Pin = @Pin,
                        HeDieuHanh = @HeDieuHanh
                       WHERE SanPhamID = @SanPhamID";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@ManHinh", ch.ManHinh ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ChipXuLy", ch.ChipXuLy ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RAM", ch.RAM ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BoNhoTrong", ch.BoNhoTrong ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Pin", ch.Pin ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HeDieuHanh", ch.HeDieuHanh ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SanPhamID", ch.SanPhamID);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public ChiTietCauHinhData()
        {
            GetAll();
        }
    }
}