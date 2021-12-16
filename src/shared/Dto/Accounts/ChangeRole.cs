namespace Shared.Dto.Accounts
{
    public record ChangeRole
    {
        public long Id { get; init; }
        public int Role { get; init; }
    }
}
