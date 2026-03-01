using MailFlow.Context;
using MailFlow.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailFlow.Controllers
{
    public class InboxController : Controller
    {
        private readonly MailContext _mailContext;
        private readonly UserManager<AppUser> _userManager;

        public InboxController(MailContext mailContext, UserManager<AppUser> userManager)
        {
            _mailContext = mailContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Inbox()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("UserLogin", "Login");

            var messages = await _mailContext.Messages
                .Where(x => x.ReceiverEmail == user.Email && x.IsDeleted == false && x.IsArchived == false)
                .OrderByDescending(x => x.SendDate)
                .ToListAsync();

            return View(messages);
        }

        public async Task<IActionResult> MessageDetail(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("UserLogin", "Login");
            }
            var message = await _mailContext.Messages.FirstOrDefaultAsync(x => x.MessageId == id && x.ReceiverEmail == user.Email);
            if (message == null)
            {
                return NotFound();
            }
            if (!message.IsStatus)
            {
                message.IsStatus = true;
                await _mailContext.SaveChangesAsync();
            }
            return View(message);
        }
        public async Task<IActionResult> ArchivedMessage(int id)
        {
            var message = await _mailContext.Messages.FindAsync(id);
            if (message != null)
            {
                message.IsArchived = true;
                await _mailContext.SaveChangesAsync();
            }
            return RedirectToAction("Inbox");

        }
        public async Task<IActionResult> ToggleStar(int id)
        {
            var message = await _mailContext.Messages.FindAsync(id);

            if (message != null)
            {
                message.IsStarred = !message.IsStarred;
                await _mailContext.SaveChangesAsync();
            }

            return RedirectToAction("Inbox");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _mailContext.Messages.FindAsync(id);
            if (message != null)
            {
                message.IsDeleted = true;
                await _mailContext.SaveChangesAsync();
            }
            return RedirectToAction("Inbox");
        }
    }
}