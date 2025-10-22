using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace LTW.Models
{
	public class VaiTro
	{
        public int VaiTroID { get; set; } 
        public string TenVaiTro { get; set; }
    }
    public class VaiTroData
    {
        public List<VaiTro> lstvaitro = new List<VaiTro>();
        public void GetAll()
        {




            using (SqlConnection con = DBConnect.GetConnection())
            {
                string sql = "SELECT * FROM vaitro";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    VaiTro vt=new VaiTro
                    {
                        VaiTroID = Convert.ToInt32(dr["vaitroid"]),
                        TenVaiTro = dr["tenvaitro"].ToString(),
                        
                    };
                    lstvaitro.Add(vt);
                }
            }


        }
        public VaiTroData()
        {
            GetAll();
        }
        
    }

}