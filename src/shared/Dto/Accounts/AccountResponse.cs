using System;

namespace Shared.Dto.Accounts
{
    public record AccountResponse
    {
        public long Id { get; init; }
        public string Title { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Role { get; init; }
        public DateTime CreateDate { get; init; }
        public DateTime? LastUpdateDate { get; init; }
        public bool IsVerified { get; init; }
    }
}