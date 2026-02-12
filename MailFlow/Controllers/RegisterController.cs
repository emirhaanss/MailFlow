using MailFlow.Dtos;
using MailFlow.Entities;
using MailFlow.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MailFlow.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailService _emailService;
        public RegisterController(UserManager<AppUser> userManager, EmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserRegisterDto userRegisterDto)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Lütfen tüm zorunlu alanları doldurun.");

            }
            if (userRegisterDto.Password != userRegisterDto.ConfirmPassword)
            {
                ModelState.AddModelError("", "Şifreler uyuşmuyor.");
                return View(userRegisterDto);
            }

            if (userRegisterDto.AcceptTerms != true)
            {
                ModelState.AddModelError("", "Kullanım şartlarını kabul etmelisiniz.");
                return View(userRegisterDto);
            }


            Random random = new Random();
            string code = random.Next(100000, 999999).ToString();
            await _emailService.SendVerificationCodeAsync(userRegisterDto.Email, "MailFlow Doğrulama Kodunuz", $"Kayıt için doğrulama kodunuz: {code}");

            //Kullanıcı bilgilerini geçici olarak session'a kaydet
            HttpContext.Session.SetString("MailCode", code);
            HttpContext.Session.SetString("Name", userRegisterDto.Name);
            HttpContext.Session.SetString("Surname", userRegisterDto.Surname);
            HttpContext.Session.SetString("Username", userRegisterDto.Username);
            HttpContext.Session.SetString("Email", userRegisterDto.Email);
            HttpContext.Session.SetString("Password", userRegisterDto.Password);

            return RedirectToAction("VerifyCode", "VerifyCode");

        }
    }
}
