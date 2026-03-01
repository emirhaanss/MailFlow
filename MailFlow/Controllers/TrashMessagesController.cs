using MailFlow.Context;
using MailFlow.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MailFlow.Controllers
{
    public class TrashMessagesController : Controller
    {
        public readonly UserManager<AppUser> _userManager;
        public readonly MailContext _mailContext;
        public TrashMessagesController(UserManager<AppUser> userManager, MailContext mailContext)
        {
            _userManager = userManager;
            _mailContext = mailContext;
        }

        public async Task<IActionResult> TrashMessage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("UserLogin", "Login");

            var trashMessages = await _mailContext.Messages
                .Where(x => x.ReceiverEmail == user.Email && x.IsDeleted)
                .OrderByDescending(x => x.SendDate)
                .ToListAsync();

            return View(trashMessages);
        }
        public async Task<IActionResult>DeletePermanently(int id)
        {
            var message = await _mailContext.Messages.FindAsync(id);
            _mailContext.Messages.Remove(message);
            await _mailContext.SaveChangesAsync();
            return RedirectToAction("TrashMessage");
        }
        public async Task<IActionResult> RestoreMessage(int id)
        {
            var message = await _mailContext.Messages.FindAsync(id);
            if (message != null)
            {
                message.IsDeleted = false;
                await _mailContext.SaveChangesAsync();
            }
            return RedirectToAction("TrashMessage");
        }
    }
}
