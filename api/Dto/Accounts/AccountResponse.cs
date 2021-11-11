using System;
using WebApi.Entities.Accounts;

namespace WebApi.Dto.Accounts
{
    public class AccountResponse
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


        public static AccountResponse CreateFromModel(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            return new AccountResponse
            {
                Id = account.Id,
                CreateDate = account.CreateDate,
                Email = account.Email,
                FirstName = account.Name.FirstName,
                LastName = account.Name.LastName,
                Title = account.Name.Title,
                IsVerified = account.IsVerified,
                Role = account.Role.ToString(),
                LastUpdateDate = account.LastUpdateDate
            };
        }
    }
}