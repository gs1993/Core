using System.ComponentModel.DataAnnotations;

namespace Shared.Dto.Accounts
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}