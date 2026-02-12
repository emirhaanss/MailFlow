using MailFlow.Context;
using MailFlow.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MailFlow.Controllers
{
    public class MessageController : Controller
    {
        private readonly MailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MessageController(MailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateMessage()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateMessage(Message message)
        {
            message.SendDate = DateTime.Now;
            message.IsStatus = false;
            _context.Messages.Add(message);
            _context.SaveChanges();
            return RedirectToAction("Sendbox");
        }
        public async Task<IActionResult> Inbox()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messageList = _context.Messages.Where(x => x.ReceiverEmail == user.Email).ToList();
            return View(messageList);
        }
    }
}
