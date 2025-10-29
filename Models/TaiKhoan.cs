using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LTW.Models
{
	public class TaiKhoan
	{
        public int TaiKhoanID { get; set; }
        public string TenDangNhap { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public int VaiTroID { get; set; }

        public string Avatar { get; set; }





    }
    public class TaiKhoanData
    {
        public List<TaiKhoan> dsTaiKhoan = new List<TaiKhoan>();
        public void GetAll()
        {




            using (SqlConnection con = DBConnect.GetConnection())
            {
                string sql = "SELECT * FROM TaiKhoan";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    TaiKhoan tk = new TaiKhoan
                    {
                        TaiKhoanID = Convert.ToInt32(dr["TaiKhoanID"]),
                        TenDangNhap = dr["TenDangNhap"].ToString(),
                        HoTen = dr["HoTen"].ToString(),
                        Email = dr["Email"].ToString(),
                        MatKhau = dr["MatKhau"].ToString(),
                        SoDienThoai = dr["SoDienThoai"].ToString(),
                        DiaChi = dr["DiaChi"].ToString(),
                        VaiTroID = Convert.ToInt32(dr["VaiTroID"]),
                        Avatar = dr["Avatar"].ToString()
                    };
                    dsTaiKhoan.Add(tk);
                }
            }


        }
        public TaiKhoanData()
        {
            GetAll();
        }
        public bool TaoTaiKhoan(TaiKhoan tk)
        {
            using (SqlConnection con = DBConnect.GetConnection())
            {
                string sql = @"INSERT INTO TaiKhoan 
                        (TenDangNhap, HoTen, Email, MatKhau, SoDienThoai, DiaChi, VaiTroID)
                       VALUES 
                        (@TenDangNhap, @HoTen, @Email, @MatKhau, @SoDienThoai, @DiaChi, @VaiTroID)";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@TenDangNhap", tk.TenDangNhap);
                    cmd.Parameters.AddWithValue("@HoTen", tk.HoTen ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", tk.Email);
                    cmd.Parameters.AddWithValue("@MatKhau", tk.MatKhau);
                    cmd.Parameters.AddWithValue("@SoDienThoai", tk.SoDienThoai ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChi", tk.DiaChi ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VaiTroID", tk.VaiTroID);
                    cmd.Parameters.AddWithValue("@Avatar", tk.Avatar ?? (object)DBNull.Value);

                    con.Open();
                    try
                    {
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                    catch (SqlException ex)
                    {

                        if (ex.Number == 2627)
                            return false;
                        else
                            throw;
                    }
                }
            }
        }
    }
}