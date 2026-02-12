using System.ComponentModel.DataAnnotations;

namespace MailFlow.Dtos
{
    public class UserRegisterDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
        public string Username { get; set; }

        [Required(ErrorMessage = "Sözleşmeyi kabul etmelisiniz")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Sözleşmeyi kabul etmelisiniz")]
        public bool AcceptTerms { get; set; }
    }
}
