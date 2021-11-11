using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Entities.Accounts
{
    public class Account : BaseEntity
    {
        public virtual Name Name { get; private set; }
        public Email Email { get; private set; }
        public virtual Password Password { get; private set; }
        public Role Role { get; private set; }
        public string VerificationToken { get; private set; }
        public DateTime? Verified { get; private set; }
        public string ResetToken { get; private set; }
        public DateTime? ResetTokenExpires { get; private set; }
        public DateTime? PasswordReset { get; private set; }


        private readonly List<RefreshToken> _refreshTokens = new();
        public virtual IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens;


        protected Account() { }
        private Account(Name name, Email email, Password password, Role role, string verificationToken, DateTime createDate) : base(createDate)
        {
            Name = name;
            Email = email;
            Password = password;
            Role = role;
            VerificationToken = verificationToken;
        }

        public static Result<Account> Create(Name name, Email email, Password password, Role role, string verificationToken, DateTime created)
        {
            return Result.Success(new Account(name, email, password, role, verificationToken, created));
        }


        public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;
        public bool OwnsToken(string token) => RefreshTokens.FirstOrDefault(x => x.Token == token) != null;

        public void AddRefreshTokenAndRemoveOldTokens(RefreshToken refreshToken, DateTime dateTimeNow, int refreshTokenTtlInDays)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));
            if (refreshTokenTtlInDays < 0)
                throw new ArgumentNullException(nameof(refreshTokenTtlInDays));

            _refreshTokens.Add(refreshToken);
            _refreshTokens.RemoveAll(x => !x.IsActive && x.Created.AddDays(refreshTokenTtlInDays) <= dateTimeNow);
        }

        public void AddResetToken(string resetToken, DateTime tokenExpireDate)
        {
            ResetToken = resetToken;
            ResetTokenExpires = tokenExpireDate;
        }

        public void SetVerificationSucceded(DateTime verificationDate)
        {
            Verified = verificationDate;
            VerificationToken = null;
        }

        public void ResetPassword(Password newPasswordHash, DateTime passwordResetDate)
        {
            Password = newPasswordHash;
            PasswordReset = passwordResetDate;
            ResetToken = null;
            ResetTokenExpires = null;
        }

        public void Update(Name name, DateTime updateDate)
        {
            if (name != null && Name != name)
            {
                Name = name;
                LastUpdateDate = updateDate;
            }
        }

        public void ChangeRole(Role role)
        {
            Role = role;
        }
    }
}