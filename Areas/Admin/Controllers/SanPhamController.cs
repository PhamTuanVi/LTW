using LTW.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTW.Areas.Admin.Controllers
{
    public class SanPhamController : Controller
    {
        [HttpGet]
        public ActionResult ThemSanPham()
        {
            DanhMucData dmData = new DanhMucData();
            ViewBag.DanhMucList = dmData.GetDanhMucList();
            return View();
        }
        [HttpPost]
        public ActionResult ThemSanPham(SanPham sp, ChiTietCauHinh ch, HttpPostedFileBase fileAnh)
        {
            DanhMucData dmData = new DanhMucData();
            ViewBag.DanhMucList = dmData.GetDanhMucList(sp.DanhMucID);
            if (ModelState.IsValid)
            {
                try
                {
                    string fileName = null;
                    if (fileAnh != null && fileAnh.ContentLength > 0)
                    {
                        fileName = System.IO.Path.GetFileName(fileAnh.FileName);
                        string path = Server.MapPath("~/Content/Images/" + fileName);
                        fileAnh.SaveAs(path);
                    }

                    using (SqlConnection con = DBConnect.GetConnection())
                    {
                        con.Open();
                        SqlTransaction tran = con.BeginTransaction();

                        try
                        {

                            string sqlSP = @"
                                INSERT INTO SanPham (TenSanPham, ThuongHieu, MoTa, SoLuong, Gia, HinhAnh1, NgayThem, DanhMucID)
                                VALUES (@TenSanPham, @ThuongHieu, @MoTa, @SoLuong, @Gia, @HinhAnh1, GETDATE(), @DanhMucID);
                                SELECT SCOPE_IDENTITY();";

                            SqlCommand cmdSP = new SqlCommand(sqlSP, con, tran);
                            cmdSP.Parameters.AddWithValue("@TenSanPham", sp.TenSanPham);
                            cmdSP.Parameters.AddWithValue("@ThuongHieu", sp.ThuongHieu);
                            cmdSP.Parameters.AddWithValue("@MoTa", sp.MoTa ?? "");
                            cmdSP.Parameters.AddWithValue("@SoLuong", sp.SoLuong);
                            cmdSP.Parameters.AddWithValue("@Gia", sp.Gia);
                            cmdSP.Parameters.AddWithValue("@HinhAnh1", fileName ?? "");
                            cmdSP.Parameters.AddWithValue("@DanhMucID", sp.DanhMucID);
                            int newSanPhamID = Convert.ToInt32(cmdSP.ExecuteScalar());

                            
                            string sqlCH = @"INSERT INTO ChiTietCauHinh 
                                             (SanPhamID, ManHinh, ChipXuLy, RAM, BoNhoTrong, Pin, HeDieuHanh)
                                             VALUES (@SanPhamID, @ManHinh, @ChipXuLy, @RAM, @BoNhoTrong, @Pin, @HeDieuHanh)";
                            SqlCommand cmdCH = new SqlCommand(sqlCH, con, tran);
                            cmdCH.Parameters.AddWithValue("@SanPhamID", newSanPhamID);
                            cmdCH.Parameters.AddWithValue("@ManHinh", ch.ManHinh);
                            cmdCH.Parameters.AddWithValue("@ChipXuLy", ch.ChipXuLy);
                            cmdCH.Parameters.AddWithValue("@RAM", ch.RAM);
                            cmdCH.Parameters.AddWithValue("@BoNhoTrong", ch.BoNhoTrong);
                            cmdCH.Parameters.AddWithValue("@Pin", ch.Pin);
                            cmdCH.Parameters.AddWithValue("@HeDieuHanh", ch.HeDieuHanh);
                            cmdCH.ExecuteNonQuery();

                            tran.Commit();
                            ViewBag.Message = "✅ Thêm sản phẩm thành công!";
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            ViewBag.Error = "❌ Lỗi khi thêm: " + ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "❌ Lỗi xử lý: " + ex.Message;
                }
            }
            return View();
        }
        public ActionResult Index()
        {
            SanPhamData dt= new SanPhamData();
            var lstsp = dt.dsSanPham;
            return View(lstsp);
        }
        public ActionResult ChiTiet(int id)
        {
            SanPhamData spData = new SanPhamData();
            var sp = spData.dsSanPham.FirstOrDefault(x => x.SanPhamID == id);

            if (sp == null)
            {
                return HttpNotFound();
            }    
               

            return View(sp);
        }

        public PartialViewResult CauHinhPartial(int id)
        {
            ChiTietCauHinhData chData = new ChiTietCauHinhData();
            var ch = chData.GetBySanPhamID(id);

            return PartialView("CauHinhPartial", ch);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaSanPham(int id)
        {
            using (SqlConnection con = DBConnect.GetConnection())
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    string sqlGioHang = "DELETE FROM GioHangChiTiet WHERE SanPhamID = @id";
                    SqlCommand cmdGioHang = new SqlCommand(sqlGioHang, con, tran);
                    cmdGioHang.Parameters.AddWithValue("@id", id);
                    cmdGioHang.ExecuteNonQuery();

                    string sqlCTDH = "DELETE FROM ChiTietDonHang WHERE SanPhamID = @SanPhamID";
                    SqlCommand cmdCTDH = new SqlCommand(sqlCTDH, con, tran);
                    cmdCTDH.Parameters.AddWithValue("@SanPhamID", id);
                    cmdCTDH.ExecuteNonQuery();

                    string sqlCH = "DELETE FROM ChiTietCauHinh WHERE SanPhamID = @SanPhamID";
                    SqlCommand cmdCH = new SqlCommand(sqlCH, con, tran);
                    cmdCH.Parameters.AddWithValue("@SanPhamID", id);
                    cmdCH.ExecuteNonQuery();

                   
                    string sqlSP = "DELETE FROM SanPham WHERE SanPhamID = @SanPhamID";
                    SqlCommand cmdSP = new SqlCommand(sqlSP, con, tran);
                    cmdSP.Parameters.AddWithValue("@SanPhamID", id);
                    int rowsAffected = cmdSP.ExecuteNonQuery();

                  
                    tran.Commit();

                    if (rowsAffected > 0)
                    {
                        TempData["Message"] = "✅ Đã xóa sản phẩm thành công!";
                    }
                    else
                    {
                        TempData["Error"] = "⚠️ Không tìm thấy sản phẩm cần xóa!";
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    TempData["Error"] = "❌ Lỗi khi xóa sản phẩm: " + ex.Message;
                }
            }

            
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult SuaSanPham(int id)
        {
            SanPhamData spData = new SanPhamData();
            ChiTietCauHinhData chData = new ChiTietCauHinhData();
            DanhMucData dmData = new DanhMucData();
            var sp = spData.dsSanPham.FirstOrDefault(x => x.SanPhamID == id);
            var ch = chData.GetBySanPhamID(id);

            if (sp == null)
                return HttpNotFound();

            ViewBag.CauHinh = ch;
            ViewBag.DanhMucList = dmData.GetDanhMucList(sp.DanhMucID); 
            return View(sp);
        }


        [HttpPost]
        public ActionResult SuaSanPham(SanPham sp, ChiTietCauHinh ch, HttpPostedFileBase fileAnh)
        {
            DanhMucData dmData = new DanhMucData();
            ViewBag.DanhMucList = dmData.GetDanhMucList(sp.DanhMucID);
            ViewBag.CauHinh = ch;

            if (ModelState.IsValid)
            {
                using (SqlConnection con = DBConnect.GetConnection())
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    try
                    {
                        string fileName = sp.HinhAnh1;
                        if (fileAnh != null && fileAnh.ContentLength > 0)
                        {
                            fileName = System.IO.Path.GetFileName(fileAnh.FileName);
                            string path = Server.MapPath("~/Content/Images/" + fileName);
                            fileAnh.SaveAs(path);
                        }

                        // 🟢 Cập nhật DanhMucID
                        string sqlSP = @"
                            UPDATE SanPham 
                            SET TenSanPham = @TenSanPham,
                                ThuongHieu = @ThuongHieu,
                                MoTa = @MoTa,
                                SoLuong = @SoLuong,
                                Gia = @Gia,
                                HinhAnh1 = @HinhAnh1,
                                DanhMucID = @DanhMucID
                            WHERE SanPhamID = @SanPhamID";

                        SqlCommand cmdSP = new SqlCommand(sqlSP, con, tran);
                        cmdSP.Parameters.AddWithValue("@TenSanPham", sp.TenSanPham);
                        cmdSP.Parameters.AddWithValue("@ThuongHieu", sp.ThuongHieu);
                        cmdSP.Parameters.AddWithValue("@MoTa", sp.MoTa ?? "");
                        cmdSP.Parameters.AddWithValue("@SoLuong", sp.SoLuong);
                        cmdSP.Parameters.AddWithValue("@Gia", sp.Gia);
                        cmdSP.Parameters.AddWithValue("@HinhAnh1", fileName ?? "");
                        cmdSP.Parameters.AddWithValue("@DanhMucID", sp.DanhMucID);
                        cmdSP.Parameters.AddWithValue("@SanPhamID", sp.SanPhamID);
                        cmdSP.ExecuteNonQuery();

                        string sqlCH = @"
                            UPDATE ChiTietCauHinh
                            SET ManHinh = @ManHinh,
                                ChipXuLy = @ChipXuLy,
                                RAM = @RAM,
                                BoNhoTrong = @BoNhoTrong,
                                Pin = @Pin,
                                HeDieuHanh = @HeDieuHanh
                            WHERE SanPhamID = @SanPhamID";

                        SqlCommand cmdCH = new SqlCommand(sqlCH, con, tran);
                        cmdCH.Parameters.AddWithValue("@ManHinh", ch.ManHinh);
                        cmdCH.Parameters.AddWithValue("@ChipXuLy", ch.ChipXuLy);
                        cmdCH.Parameters.AddWithValue("@RAM", ch.RAM);
                        cmdCH.Parameters.AddWithValue("@BoNhoTrong", ch.BoNhoTrong);
                        cmdCH.Parameters.AddWithValue("@Pin", ch.Pin);
                        cmdCH.Parameters.AddWithValue("@HeDieuHanh", ch.HeDieuHanh);
                        cmdCH.Parameters.AddWithValue("@SanPhamID", sp.SanPhamID);
                        cmdCH.ExecuteNonQuery();

                        tran.Commit();
                        TempData["Message"] = "✅ Cập nhật sản phẩm thành công!";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        ViewBag.Error = "❌ Lỗi khi cập nhật: " + ex.Message;
                    }
                }
            }
            return View(sp);
        }
    }
}