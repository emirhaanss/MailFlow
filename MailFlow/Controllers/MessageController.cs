using MailFlow.Context;
using MailFlow.Dtos;
using MailFlow.Entities;
using MailFlow.Settings;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MailFlow.Controllers
{
    public class MessageController : Controller
    {
        private readonly MailContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailSettings _emailSettings;

        public MessageController(
            MailContext context,
            UserManager<AppUser> userManager,
            IOptions<EmailSettings> emailSettings)
        {
            _context = context;
            _userManager = userManager;
            _emailSettings = emailSettings.Value;
        }

        [HttpGet]
        public IActionResult CreateMessage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(MailRequestDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Lütfen giriş yapın.";
                return RedirectToAction("UserLogin", "Login");
            }

            // DB kaydı
            var message = new Message
            {
                SenderEmail = user.Email, // Sabit SMTP maili
                AppUserId = user.Id,
                ReceiverEmail = dto.ReceiverEmail,
                Subject = dto.Subject,
                MessageDetail = dto.MessageDetail,
                SendDate = DateTime.Now,
                IsStatus = false,
                IsStarred = false,
                IsArchived = false,
                IsDeleted = false
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            // --- Sabit SMTP ile mail gönderimi ---
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("MailFlow", _emailSettings.Email));
            mimeMessage.To.Add(new MailboxAddress("User", dto.ReceiverEmail));
            mimeMessage.Subject = dto.Subject;
            mimeMessage.Body = new BodyBuilder { TextBody = dto.MessageDetail }.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                smtp.Connect(_emailSettings.Host, _emailSettings.Port, false);
                smtp.Authenticate(_emailSettings.Email, _emailSettings.Password);
                smtp.Send(mimeMessage);
                smtp.Disconnect(true);
            }

            return RedirectToAction("Inbox","Inbox");
        }
    }
}