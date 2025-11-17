using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
namespace LTW.Services
{
	public class MailService
	{
        public static void SendMail(string toEmail, string subject, string body, bool isHtml = false)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Website LTW", ConfigurationManager.AppSettings["EmailUserName"]));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

           
                email.Body = new TextPart(isHtml ? MimeKit.Text.TextFormat.Html : MimeKit.Text.TextFormat.Plain)
                {
                    Text = body
                };

                using (var smtp = new SmtpClient())
                {
                   
                    smtp.Connect(ConfigurationManager.AppSettings["EmailHost"],
                                 int.Parse(ConfigurationManager.AppSettings["EmailPort"]),
                                 SecureSocketOptions.StartTls);

                   
                    smtp.Authenticate(ConfigurationManager.AppSettings["EmailUserName"],
                                      ConfigurationManager.AppSettings["EmailPassword"]);

                    smtp.Send(email);
                    smtp.Disconnect(true);
                }

                Console.WriteLine("✅ Gửi mail thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi gửi mail: " + ex.Message);
                //throw;
            }
        }
    }
}