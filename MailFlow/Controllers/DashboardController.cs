using MailFlow.Context;
using MailFlow.Dtos;
using MailFlow.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailFlow.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly MailContext _context;
        public DashboardController(UserManager<AppUser> userManager, MailContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> DashboardPage()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("UserLogin", "Login");

            var userMessages = await _context.Messages
                .Where(x => x.ReceiverEmail == user.Email)
                .ToListAsync();

            var model = new DashboardDto
            {
                TotalMessages = userMessages.Count,
                StarredMessages = userMessages.Count(x => x.IsStarred),
                ArchivedMessages = userMessages.Count(x => x.IsArchived),
                UnreadMessages = userMessages.Count(x => !x.IsStatus),
                FullName = user.Name + " " + user.Surname,
                Email = user.Email,
                ImageUrl = user.ImageUrl
            };

            return View(model);
        }
    }
}
