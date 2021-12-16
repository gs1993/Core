using System.ComponentModel.DataAnnotations;

namespace Shared.Dto.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}