using MailFlow.Dtos;
using MailFlow.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MailFlow.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;

        public LoginController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginDto userLoginDto)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Lütfen tüm alanları doldurun");
                return View(userLoginDto);
            }
            var result = await _signInManager.PasswordSignInAsync(userLoginDto.Username, userLoginDto.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("DashboardPage", "Dashboard");
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı");
                return View(userLoginDto);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Kullanıcıyı oturumdan çıkart
            await _signInManager.SignOutAsync();

            // Login sayfasına yönlendir
            return RedirectToAction("UserLogin", "Login");
        }
    }
}
