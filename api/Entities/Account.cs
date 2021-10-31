using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Entities
{
    public class Account
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; } //TODO: add salt prop
        public bool AcceptTerms { get; private set; } //TODO: remove
        public Role Role { get; private set; }
        public string VerificationToken { get; private set; }
        public DateTime? Verified { get; private set; }
        public string ResetToken { get; private set; }
        public DateTime? ResetTokenExpires { get; private set; }
        public DateTime? PasswordReset { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime? Updated { get; private set; }

        private readonly List<RefreshToken> _refreshTokens;
        public virtual IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens;


        protected Account() { }
        private Account(string title, string firstName, string lastName, string email, string passwordHash, bool acceptTerms, Role role, string verificationToken, DateTime created)
        {
            Title = title;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordHash = passwordHash;
            AcceptTerms = acceptTerms;
            Role = role;
            VerificationToken = verificationToken;
            Created = created;
        }

        public static Result<Account> Create(string title, string firstName, string lastName, string email, string passwordHash, bool acceptTerms, Role role, string verificationToken, DateTime created)
        {
            //TODO: validatiom

            return Result.Success(new Account(title, firstName, lastName, email, passwordHash, acceptTerms, role, verificationToken, created));
        }


        public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;
        public bool OwnsToken(string token) => RefreshTokens.FirstOrDefault(x => x.Token == token) != null;
        
        public void AddRefreshTokenAndRemoveOldTokens(RefreshToken refreshToken, DateTime dateTimeNow, int refreshTokenTtlInDays)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));
            if(refreshTokenTtlInDays < 0)
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

        public void ResetPassword(string newPasswordHash, DateTime passwordResetDate)
        {
            PasswordHash = newPasswordHash;
            PasswordReset = passwordResetDate;
            ResetToken = null;
            ResetTokenExpires = null;
        }

        public void Update(string title, string firstName, string lastName, DateTime updateDate)
        {
            if (!string.IsNullOrWhiteSpace(title))
                Title = title;
            if (!string.IsNullOrWhiteSpace(firstName))
                FirstName = firstName;
            if (!string.IsNullOrWhiteSpace(lastName))
                LastName = lastName;
            if (!string.IsNullOrWhiteSpace(title) || !string.IsNullOrWhiteSpace(firstName) || !string.IsNullOrWhiteSpace(lastName))
                Updated = updateDate;
        }
    }
}