using MailFlow.Dtos;
using MailFlow.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MailFlow.Controllers
{
    public class VerifyCodeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public VerifyCodeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult VerifyCode()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VerifyCode(VerifyCodeDto verifyCodeDto)
        {
            //Sessiondaki kodu al
            var sessionCode = HttpContext.Session.GetString("MailCode");
            if (sessionCode == null)
            {
                ModelState.AddModelError("", "Kod bulunamadı. Lütfen tekrar deneyin.");
                return View(verifyCodeDto);
            }
            if (sessionCode != verifyCodeDto.Code)
            {
                ModelState.AddModelError("", "Kod yanlış. Lütfen tekrar deneyin.");
                return View(verifyCodeDto);
            }
            //Session’dan kullanıcı bilgilerini çek
            AppUser appUser = new AppUser
            {
                Name = HttpContext.Session.GetString("Name"),
                Surname = HttpContext.Session.GetString("Surname"),
                Email = HttpContext.Session.GetString("Email"),
                UserName = HttpContext.Session.GetString("Username")
            };
            string passwaord = HttpContext.Session.GetString("Password");

            var result = await _userManager.CreateAsync(appUser, passwaord);

            if (result.Succeeded)
            {
                //Session temizle
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();

            }

        }
    }
}
