using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTW.Models;
namespace LTW.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DateTime ngayHienTai = DateTime.Now;
            var sp = new SanPhamData();
            var lstsp = sp.dsSanPham.OrderBy(s=>s.SanPhamID).ToList();
            var sanPhamMoi = lstsp
                   .Where(x => x.NgayThem >= ngayHienTai.AddDays(-14))
                   .Take(8) 
                   .ToList();
            var dm = new DanhMucData();
            var dmlst = dm.dsDanhMuc.OrderBy(d => d.TenDanhMuc).ToList();
            ViewBag.SanPhamMoi = sanPhamMoi;
            ViewBag.DanhSachDanhMuc = dmlst;
            return View(lstsp); 
        }
        //public ActionResult TenThuongHieu(int id)
        //{

        //    var sp = new SanPhamData();
        //    var lstsp = sp.dsSanPham.ToList();
        //    var thuonghieu = lstsp.Where(th => th.DanhMucID == id).ToList();
        //    return PartialView(thuonghieu);
        //}
        public ActionResult ThuongHieuTheoDanhMuc(int id)
        {
            var spData = new SanPhamData();
            var lstSp = spData.dsSanPham.Where(sp => sp.DanhMucID == id).ToList();

            
            var thuongHieus = lstSp
                .Select(sp => sp.ThuongHieu)
                .Distinct()
                .ToList();

            ViewBag.DanhMucID = id;
            return PartialView(thuongHieus);
        }
        public ActionResult SanPhamTheoDanhMucPartial(int id)
        {
            var spData = new SanPhamData();
            var lstSp = spData.dsSanPham
                .Where(sp => sp.DanhMucID == id)
                .ToList();

            return PartialView(lstSp);
        }
        public ActionResult LocTheoDanhMuc(int id)
        {
            var sp = new SanPhamData();
            var lstdm = sp.dsSanPham.Where(s => s.DanhMucID == id).ToList();
            return View(lstdm);
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            var sp = new SanPhamData();
            var lstsp = sp.dsSanPham.ToList();
            var lstsptim = lstsp
                    .Where(spt => spt.TenSanPham.ToLower().Contains(keyword.ToLower()))
                    .ToList();
            return View(lstsptim );
        }
        public ActionResult LocSanPham(string thuongHieu, decimal? minGia, decimal? maxGia, string sort)
        {
            var spData = new SanPhamData();
            var lstSp = spData.dsSanPham.ToList();

            if (string.IsNullOrEmpty(thuongHieu) && !minGia.HasValue && !maxGia.HasValue && string.IsNullOrEmpty(sort))
                {
                        ViewBag.ThuongHieuSelectList = new SelectList(spData.dsSanPham.Select(p => p.ThuongHieu).Distinct().ToList());
                        ViewBag.SortSelectList = new List<SelectListItem>
                    {
                    new SelectListItem { Text = "Giá tăng dần", Value = "asc" },
                    new SelectListItem { Text = "Giá giảm dần", Value = "desc" }
                    };

        
                    return PartialView(new List<SanPham>());
                }
            if (!string.IsNullOrEmpty(thuongHieu))
            {
                lstSp = lstSp.Where(p => p.ThuongHieu == thuongHieu).ToList();
            }

            
            if (minGia.HasValue)
            {
                lstSp = lstSp.Where(p => p.Gia >= minGia.Value).ToList();
            }
                
            if (maxGia.HasValue)
            {
                lstSp = lstSp.Where(p => p.Gia <= maxGia.Value).ToList();
            }
               

            
            if (!string.IsNullOrEmpty(sort))
            {
                if (sort == "asc")
                {
                    lstSp = lstSp.OrderBy(p => p.Gia).ToList();
                }
                    
                else if (sort == "desc")
                {
                    lstSp = lstSp.OrderByDescending(p => p.Gia).ToList();
                }
                    
            }

            
            var thuongHieuList = spData.dsSanPham.Select(p => p.ThuongHieu).Distinct().ToList();
            ViewBag.ThuongHieuSelectList = new SelectList(thuongHieuList, thuongHieu);

           
            ViewBag.SortSelectList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Giá tăng dần", Value = "asc", Selected = sort=="asc"},
                new SelectListItem { Text = "Giá giảm dần", Value = "desc", Selected = sort=="desc"}
            };

            return View(lstSp);
        }
    }
}