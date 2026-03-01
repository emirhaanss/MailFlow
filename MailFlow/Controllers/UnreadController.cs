using MailFlow.Context;
using MailFlow.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MailFlow.Controllers
{
    public class UnreadController : Controller
    {
        private readonly MailContext _context;
        private readonly UserManager<AppUser> _userManager;
        public UnreadController(MailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> UnreadMessages()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("UserLogin", "Login");
            }
            var unreadMessages = _context.Messages.Where(m => !m.IsStatus && m.ReceiverEmail == user.Email).OrderByDescending(x => x.SendDate).ToList();
            return View(unreadMessages);
        }
    }
}
