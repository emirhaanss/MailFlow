using MailFlow.Context;
using MailFlow.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailFlow.Controllers
{
    public class SentMessageController : Controller
    {
        private readonly MailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public SentMessageController(UserManager<AppUser> user, MailContext context)
        {
            _userManager = user;
            _context = context;
        }

        public async Task<IActionResult> SentMessages()
        {
            // Login olan kullanıcıyı al
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("UserLogin", "Login"); // Kullanıcı login değilse
            }

            // Sadece bu kullanıcının gönderdiği mesajlar
            var sentMessages = await _context.Messages
                .Where(m => m.AppUserId == user.Id)
                .OrderByDescending(x => x.SendDate) // Gönderim tarihine göre sırala
                .ToListAsync();

            return View(sentMessages);

        }
        public async Task<IActionResult> MessageDetail(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("UserLogin", "Login"); // Kullanıcı login değilse
            }
            var message = await _context.Messages.FirstOrDefaultAsync(x => x.MessageId == id && x.SenderEmail == user.Email);
            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Remove(message);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("SentMessages");
        }
    }
}