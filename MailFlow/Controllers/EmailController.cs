using MailFlow.Dtos;
using MailFlow.Settings;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MailFlow.Controllers
{
    public class EmailController : Controller
    {
        private readonly EmailSettings _emailSettings;

        public EmailController(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        [HttpGet]
        public IActionResult SendEmail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendEmail(MailRequestDto mailRequestDto)
        {
            MimeMessage mimeMessage = new MimeMessage();

            // Gönderen
            mimeMessage.From.Add(new MailboxAddress("MailFlow", _emailSettings.Email));

            // Alıcı
            mimeMessage.To.Add(new MailboxAddress("User", mailRequestDto.ReceiverEmail));

            // Konu
            mimeMessage.Subject = mailRequestDto.Subject;

            // Mail içeriği
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = mailRequestDto.MessageDetail;
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            // SMTP bağlantısı
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Connect(_emailSettings.Host, _emailSettings.Port, false);
            smtpClient.Authenticate(_emailSettings.Email, _emailSettings.Password);
            smtpClient.Send(mimeMessage);
            smtpClient.Disconnect(true);

            return RedirectToAction("UserList");
        }
    }
}
