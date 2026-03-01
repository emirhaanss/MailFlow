using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MailFlow.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using MailFlow.Entities;

namespace MailFlow.Controllers
{
    public class StarredMessagesController : Controller
    {
        private readonly MailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StarredMessagesController(MailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> StarredMessage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("UserLogin", "Login");
            }

            var starredMessages = await _context.Messages.Where(m => m.IsStarred && m.ReceiverEmail == user.Email).OrderByDescending(x => x.SendDate).ToListAsync();
            return View(starredMessages);
        }
        public async Task<IActionResult> UnStarredMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                message.IsStarred = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("StarredMessage");
        }
    }
}
