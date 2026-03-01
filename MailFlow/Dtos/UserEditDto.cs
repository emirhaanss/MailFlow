using System.ComponentModel.DataAnnotations;

namespace MailFlow.Dtos
{
    public class UserEditDto
    {

        [Required(ErrorMessage = "Ad alanı boş olamaz")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyad alanı boş olamaz")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email alanı boş olamaz")]
        [EmailAddress(ErrorMessage = "Geçerli bir email girin")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor")]
        public string ConfirmPassword { get; set; }

        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
    }
}
