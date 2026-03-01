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

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) return RedirectToAction("Login", "Login");

            UserEditDto userEditDto = new UserEditDto
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                ImageUrl = user.ImageUrl
            };
            return View(userEditDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(UserEditDto userEditDto)
        {
            // 1. Kullanıcıyı bul
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            // 2. Şifre Doğrulama (Confirm Password Kontrolü)
            if (userEditDto.Password != userEditDto.ConfirmPassword)
            {
                ModelState.AddModelError("", "Girdiğiniz şifreler birbiriyle uyuşmuyor!");
                return View(userEditDto);
            }

            // 3. Temel Bilgileri Güncelle
            user.Name = userEditDto.Name;
            user.Surname = userEditDto.Surname;
            user.Email = userEditDto.Email;

            // 4. Şifre Güncelleme (Sadece şifre alanları doluysa)
            if (!string.IsNullOrEmpty(userEditDto.Password))
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userEditDto.Password);
            }

            // 5. Resim Yükleme İşlemi (Sadece yeni resim seçildiyse)
            if (userEditDto.Image != null)
            {
                var resource = Directory.GetCurrentDirectory();
                var extension = Path.GetExtension(userEditDto.Image.FileName);
                var imageName = Guid.NewGuid() + extension;
                var saveLocation = Path.Combine(resource, "wwwroot/images/", imageName);

                // 'using' kullanımı dosyanın işlem bittikten sonra serbest bırakılmasını sağlar
                using (var stream = new FileStream(saveLocation, FileMode.Create))
                {
                    await userEditDto.Image.CopyToAsync(stream);
                }
                user.ImageUrl = imageName;
            }

            // 6. Veritabanını Güncelle
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Inbox", "Message");
            }

            // Identity hatalarını ModelState'e ekle (Örn: Email zaten alınmış vb.)
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(userEditDto);
        }
    }
}