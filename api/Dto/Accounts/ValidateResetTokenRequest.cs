using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}