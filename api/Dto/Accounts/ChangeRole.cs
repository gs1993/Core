using WebApi.Entities.Accounts;

namespace WebApi.Dto.Accounts
{
    public record ChangeRole
    {
        public long Id { get; init; }
        public Role Role { get; init; }
    }
}
