using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto.Accounts
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}