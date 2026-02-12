using System.ComponentModel.DataAnnotations;

namespace MailFlow.Dtos
{
    public class VerifyCodeDto
    {
        [Required(ErrorMessage = "Kod boş bırakılamaz")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Kod 6 haneli olmalı")]
        public string Code { get; set; }
    }
}
