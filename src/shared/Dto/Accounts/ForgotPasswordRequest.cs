using System.ComponentModel.DataAnnotations;

namespace Shared.Dto.Accounts
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}