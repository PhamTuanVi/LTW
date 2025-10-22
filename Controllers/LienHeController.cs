using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTW.Services;

namespace LTW.Controllers
{
    public class LienHeController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GuiLienHe(string hoTen, string noiDung)
        {

            if (Session["email"] == null)
            {
               
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            string emailNguoiGui = Session["email"].ToString();
            
                string subject = $"📩 Liên hệ từ khách hàng: {hoTen}";
                string body = $@"
                    <h3>Thông tin khách hàng:</h3>
                    <p><b>Họ tên:</b> {hoTen}</p>
                    <p><b>Email:</b> {emailNguoiGui}</p>
                    <p><b>Nội dung:</b></p>
                    <p>{noiDung}</p>
                    <hr/>
                    <p>Gửi lúc: {DateTime.Now:HH:mm dd/MM/yyyy}</p>";


                MailService.SendMail("viphamaz092005@gmail.com", subject, body, true);

                TempData["Success"] = "✅ Gửi liên hệ thành công! Shop sẽ phản hồi sớm nhất.";



                return RedirectToAction("Index");
            }
         
            
        
        
    }
}