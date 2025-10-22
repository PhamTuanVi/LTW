using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTW.Models
{
	public class DonHang
	{
        public int DonHangID { get; set; }
        public int TaiKhoanID { get; set; }
        public DateTime NgayDat { get; set; }
        public int TrangThaiID { get; set; }
        public decimal TongTien { get; set; }
    }
}