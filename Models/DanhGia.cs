using System;
using System.Collections.Generic;
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
}