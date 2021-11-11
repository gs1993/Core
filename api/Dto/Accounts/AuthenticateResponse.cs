using System;
using System.Text.Json.Serialization;
using WebApi.Entities.Accounts;

namespace WebApi.Dto.Accounts
{
    public class AuthenticateResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool IsVerified { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }


        public static AuthenticateResponse CreateFromModel(Account account, string jwtToken, string refreshToken)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            return new AuthenticateResponse
            {
                Id = account.Id,
                CreateDate = account.CreateDate,
                Email = account.Email,
                FirstName = account.Name.FirstName,
                LastName = account.Name.LastName,
                Title = account.Name.Title,
                IsVerified = account.IsVerified,
                Role = account.Role.ToString(),
                LastUpdateDate = account.LastUpdateDate,
                JwtToken = jwtToken,
                RefreshToken = refreshToken
            };
        }
    }
}