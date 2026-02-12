using MailFlow.Dtos;
using MailFlow.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MailFlow.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            UserEditDto userEditDto = new UserEditDto();
            userEditDto.Name = values.Name;
            userEditDto.Surname = values.Surname;
            userEditDto.Email = values.Email;
            userEditDto.ImageUrl = values.ImageUrl;
            return View(userEditDto);
        }
        [HttpPost]
        public async Task<IActionResult> Index(UserEditDto userEditDto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            user.Name = userEditDto.Name;
            user.Surname = userEditDto.Surname;
            user.Email = userEditDto.Email;
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userEditDto.Password);

            var resource = Directory.GetCurrentDirectory();
            var extension = Path.GetExtension(userEditDto.Image.FileName);
            var imageName = Guid.NewGuid() + extension;
            var saveLocation = resource + "/wwwroot/images/" + imageName;
            var stream = new FileStream(saveLocation, FileMode.Create);
            await userEditDto.Image.CopyToAsync(stream);
            user.ImageUrl = imageName;


            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Inbox", "Message");
            }
            return View();
        }
    }
}
