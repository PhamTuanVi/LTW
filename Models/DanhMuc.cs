using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTW.Models
{
    public class DanhMuc
    {
        public int DanhMucID { get; set; }
        public string TenDanhMuc { get; set; }
        public string MoTa { get; set; }
    }
    public class DanhMucData
    {
        public List<DanhMuc> dsDanhMuc = new List<DanhMuc>();

        public void GetAll()
        {
            using (SqlConnection con = DBConnect.GetConnection())
            {
                string sql = "SELECT * FROM DanhMuc";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    DanhMuc dm = new DanhMuc
                    {
                        DanhMucID = Convert.ToInt32(dr["DanhMucID"]),
                        TenDanhMuc = dr["TenDanhMuc"].ToString(),
                        MoTa = dr["MoTa"] == DBNull.Value ? "" : dr["MoTa"].ToString()
                    };
                    dsDanhMuc.Add(dm);
                }
            }
        }

        public DanhMucData()
        {
            GetAll();
        }
        public List<SelectListItem> GetDanhMucList(int selectedId = 0)
        {
            var list = new List<SelectListItem>();
            foreach (var dm in dsDanhMuc)
            {
                list.Add(new SelectListItem
                {
                    Value = dm.DanhMucID.ToString(),
                    Text = dm.TenDanhMuc,
                    Selected = (dm.DanhMucID == selectedId)
                });
            }
            return list;
        }
    }
}