using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto.Accounts
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}