using LTW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Text;

namespace LTW.Controllers
{
    public class GioHangController : Controller
    {
        
        public ActionResult Index()
        {
            int taiKhoanId = Convert.ToInt32(Session["userid"]);
            var giohangct = new GioHangChiTietData();
            var dsgiohangchitiet = giohangct.dsGioHangChiTiet.ToList();
            var sp = new SanPhamData();
            var lstsp = sp.dsSanPham.ToList();
            var giohang = new GioHangData();
            var dsgiohang = giohang.dsGioHang.ToList();
            var giohangcanhan = dsgiohang.FirstOrDefault(gh => gh.TaiKhoanID == taiKhoanId);
            List<GioHangChiTiet> giohangchitietcanhan = new List<GioHangChiTiet>();
            if (giohangcanhan != null)
            {
                giohangchitietcanhan = dsgiohangchitiet.Where(ct => ct.GioHangID == giohangcanhan.GioHangID).ToList();
            }
            ViewBag.ChiTiets = giohangchitietcanhan;
            ViewBag.SanPhams = lstsp;
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult CapNhatSoLuong(int id, string actionType)
        {
            int taiKhoanId = Convert.ToInt32(Session["userid"]);
            var giohangct = new GioHangChiTietData();
            var dsgiohangchitiet = giohangct.dsGioHangChiTiet.ToList();
            var giohang = new GioHangData();
            var dsgiohang = giohang.dsGioHang.ToList();
            var giohangcanhan = dsgiohang.FirstOrDefault(gh => gh.TaiKhoanID == taiKhoanId);
            if (giohangcanhan != null)
            {
                var chitiet = dsgiohangchitiet.FirstOrDefault(ct => ct.GioHangID == giohangcanhan.GioHangID && ct.SanPhamID == id);
                if (chitiet != null)
                {
                    if (actionType == "plus")
                    {
                        chitiet.SoLuong++;
                    }
                    else if (actionType == "minus" && chitiet.SoLuong > 1)
                    {
                        chitiet.SoLuong--;
                    }
                    //else
                    //{

                    //}


                    using (var con = DBConnect.GetConnection())
                    {
                        string sql = @"UPDATE GioHangChiTiet 
                               SET SoLuong = @SoLuong 
                               WHERE GioHangID = @GioHangID AND SanPhamID = @SanPhamID";

                        using (var cmd = new System.Data.SqlClient.SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@SoLuong", chitiet.SoLuong);
                            cmd.Parameters.AddWithValue("@GioHangID", chitiet.GioHangID);
                            cmd.Parameters.AddWithValue("@SanPhamID", chitiet.SanPhamID);

                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    TempData["Message"] = "Cập nhật số lượng thành công!";
                }
                else
                {
                    TempData["Message"] = "Không tìm thấy sản phẩm trong giỏ hàng!";
                }
            }
            else
            {
                TempData["Message"] = "Không tìm thấy giỏ hàng của bạn!";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult ThemVaoGio(int sanPhamId, int soLuong)
        {
            int taiKhoanId = Convert.ToInt32(Session["userid"]);
            if (taiKhoanId == 0)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để thêm sản phẩm!";
                return RedirectToAction("ChiTiet", "SanPham", new { id = sanPhamId });
            }

            
            var gioHangData = new GioHangData();
            var gioHang = gioHangData.dsGioHang.FirstOrDefault(gh => gh.TaiKhoanID == taiKhoanId);

            if (gioHang == null)
            {
                
                using (var con = DBConnect.GetConnection())
                {
                    string sql = "INSERT INTO GioHang (TaiKhoanID) VALUES (@TaiKhoanID); SELECT SCOPE_IDENTITY();";
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@TaiKhoanID", taiKhoanId);
                        con.Open();
                        int newGioHangId = Convert.ToInt32(cmd.ExecuteScalar());
                        gioHang = new GioHang { GioHangID = newGioHangId, TaiKhoanID = taiKhoanId };
                    }
                }
            }

            
            using (var con = DBConnect.GetConnection())
            {
                string sql = @"INSERT INTO GioHangChiTiet (GioHangID, SanPhamID, SoLuong, NgayTao) 
                       VALUES (@GioHangID, @SanPhamID, @SoLuong, GETDATE())";
                using (var cmd = new System.Data.SqlClient.SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@GioHangID", gioHang.GioHangID);
                    cmd.Parameters.AddWithValue("@SanPhamID", sanPhamId);
                    cmd.Parameters.AddWithValue("@SoLuong", soLuong);

                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng!";
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        TempData["ErrorMessage"] = ex.Message;
                    }
                }
            }

            return RedirectToAction("ChiTiet", "SanPham", new { id = sanPhamId });
        }
        public ActionResult MiniCart()
        {
            int taiKhoanId = Convert.ToInt32(Session["userid"]);
            var giohangct = new GioHangChiTietData();
            var dsgiohangchitiet = giohangct.dsGioHangChiTiet.ToList();
            var sp = new SanPhamData();
            var lstsp = sp.dsSanPham.ToList();
            var giohang = new GioHangData();
            var dsgiohang = giohang.dsGioHang.ToList();
            var giohangcanhan = dsgiohang.FirstOrDefault(gh => gh.TaiKhoanID == taiKhoanId);
            List<GioHangChiTiet> giohangchitietcanhan = new List<GioHangChiTiet>();
            if(giohangcanhan!=null)
            {
                giohangchitietcanhan = dsgiohangchitiet.Where(ct => ct.GioHangID == giohangcanhan.GioHangID).ToList();
            }
            ViewBag.ChiTiets = giohangchitietcanhan;
            ViewBag.SanPhams = lstsp;
            return PartialView("MiniCart");
        }
        [HttpPost]
        public ActionResult XoaMotSanPham(int id)
        {
            int taiKhoanId = Convert.ToInt32(Session["userid"]);
            var giohang = new GioHangData();
            var dsgiohang = giohang.dsGioHang.ToList();
            var giohangcanhan = dsgiohang.FirstOrDefault(gh => gh.TaiKhoanID == taiKhoanId);

            using (var con = DBConnect.GetConnection())
            {
                string sql = @"DELETE FROM GioHangChiTiet 
                       WHERE GioHangID = @GioHangID AND SanPhamID = @SanPhamID";
                using (var cmd = new System.Data.SqlClient.SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@GioHangID", giohangcanhan.GioHangID);
                    cmd.Parameters.AddWithValue("@SanPhamID", id);
                    con.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy sản phẩm trong giỏ hàng!";
                    }
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult XoaTatCa()
        {
            int taiKhoanId = Convert.ToInt32(Session["userid"]);
            var giohang = new GioHangData();
            var dsgiohang = giohang.dsGioHang.ToList();
            var giohangcanhan = dsgiohang.FirstOrDefault(gh => gh.TaiKhoanID == taiKhoanId);
            using (var con = DBConnect.GetConnection())
            {
                string sql = @"DELETE FROM GioHangChiTiet 
                       WHERE GioHangID = @GioHangID";
                using (var cmd = new System.Data.SqlClient.SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@GioHangID", giohangcanhan.GioHangID);
                    
                    con.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        TempData["SuccessMessage"] = "Đã xóa toàn sản phẩm khỏi giỏ hàng!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy sản phẩm trong giỏ hàng!";
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
//Session["userid"] = nguoidung.TaiKhoanID;
//Session["username"] = nguoidung.TenDangNhap;
//Session["email"] = nguoidung.Email;
//Session["roleid"] = nguoidung.VaiTroID;
//Session["rolename"] = vaitro?.TenVaiTro ?? "User";
//Session["ten"] = nguoidung.HoTen;