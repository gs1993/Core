using CSharpFunctionalExtensions;
using System;

namespace Domain.Accounts.Entities
{
    public class RefreshToken
    {
        public int Id { get; private set; }
        public string Token { get; private set; }
        public DateTime Expires { get; private set; }
        public DateTime Created { get; private set; }
        public string CreatedByIp { get; private set; }
        public DateTime? Revoked { get; private set; }
        public string RevokedByIp { get; private set; }
        public string ReplacedByToken { get; private set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsActive => !Revoked.HasValue && !IsExpired;

        protected RefreshToken() { }
        private RefreshToken(string token, DateTime created, DateTime expires, string createdByIp)
        {
            Token = token;
            Created = created;
            Expires = expires;
            CreatedByIp = createdByIp;
        }

        public static Result<RefreshToken> Create(string token, DateTime created, DateTime expires, string createdByIp)
        {
            if (string.IsNullOrWhiteSpace(token))
                return Result.Failure<RefreshToken>("Token cannot be empty or whitespace");
            if (created == default)
                return Result.Failure<RefreshToken>("Invalid token create date");
            if (expires == default)
                return Result.Failure<RefreshToken>("Invalid token expire date");
            if (created >= expires)
                return Result.Failure<RefreshToken>("Invalid token create date");
            if (string.IsNullOrWhiteSpace(createdByIp))
                return Result.Failure<RefreshToken>("Invalid token created ip");

            return Result.Success(new RefreshToken(token, created, expires, createdByIp));
        }

        public void Revoke(DateTime revoked, string revokedByIp, string replacedByToken = null)
        {
            if (revoked == default)
                throw new ArgumentNullException(nameof(revoked));
            if (string.IsNullOrWhiteSpace(revokedByIp))
                throw new ArgumentNullException(nameof(revokedByIp));

            Revoked = revoked;
            RevokedByIp = revokedByIp;
            ReplacedByToken = string.IsNullOrWhiteSpace(replacedByToken) ? string.Empty : replacedByToken;
        }
    }
}