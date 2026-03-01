using MailFlow.Context;
using MailFlow.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailFlow.Controllers
{
    public class ArchivedMessagesController : Controller
    {
        private readonly MailContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ArchivedMessagesController(MailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> ArchivedMessage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("UserLogin", "Login");
            }
            var archivedMessages = await _context.Messages.Where(m => m.IsArchived && m.ReceiverEmail == user.Email && m.IsDeleted == false).OrderByDescending(x => x.SendDate).ToListAsync();
            return View(archivedMessages);


        }
        public async Task<IActionResult> Unarchive(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                message.IsArchived = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ArchivedMessage");
        }
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                message.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ArchivedMessage");
        }
        public async Task<IActionResult> ToggleStar(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                message.IsStarred = !message.IsStarred;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ArchivedMessage");
        }

    }
}
