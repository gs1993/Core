using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Accounts
{
    public record RegisterRequest
    {
        [Required]
        public string Title { get; init; }

        [Required]
        public string FirstName { get; init; }

        [Required]
        public string LastName { get; init; }

        [Required]
        [EmailAddress]
        public string Email { get; init; }

        [Required]
        [MinLength(6)]
        public string Password { get; init; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; init; }

        [Range(typeof(bool), "true", "true")]
        public bool AcceptTerms { get; init; }
    }
}