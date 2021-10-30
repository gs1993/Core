using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;

namespace WebApi.Entities
{
    public class Account
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public bool AcceptTerms { get; private set; }
        public Role Role { get; private set; }
        public string VerificationToken { get; private set; }
        public DateTime? Verified { get; private set; }
        public string ResetToken { get; private set; }
        public DateTime? ResetTokenExpires { get; private set; }
        public DateTime? PasswordReset { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime? Updated { get; private set; }
        public List<RefreshToken> RefreshTokens { get; private set; }


        private Account(string title, string firstName, string lastName, string email, string passwordHash, bool acceptTerms, Role role, string verificationToken,
            DateTime? verified, string resetToken, DateTime? resetTokenExpires, DateTime? passwordReset, DateTime created, DateTime? updated, List<RefreshToken> refreshTokens)
        {
            Title = title;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordHash = passwordHash;
            AcceptTerms = acceptTerms;
            Role = role;
            VerificationToken = verificationToken;
            Verified = verified;
            ResetToken = resetToken;
            ResetTokenExpires = resetTokenExpires;
            PasswordReset = passwordReset;
            Created = created;
            Updated = updated;
            RefreshTokens = refreshTokens;
        }

        private static Result<Account> Create(string title, string firstName, string lastName, string email, string passwordHash, bool acceptTerms, Role role, string verificationToken,
            DateTime? verified, string resetToken, DateTime? resetTokenExpires, DateTime? passwordReset, DateTime created, DateTime? updated, List<RefreshToken> refreshTokens)
        {
            //TODO: validatiom

            return Result.Success(new Account(title, firstName, lastName, email, passwordHash, acceptTerms, role, verificationToken, verified, resetToken, resetTokenExpires, passwordReset, created, updated, refreshTokens));
        }


        public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;
        public bool OwnsToken(string token) => RefreshTokens?.Find(x => x.Token == token) != null;
        
        public void AddRefreshTokenAndRemoveOldTokens(RefreshToken refreshToken, DateTime dateTimeNow, int refreshTokenTtlInDays)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));
            if(refreshTokenTtlInDays < 0)
                throw new ArgumentNullException(nameof(refreshTokenTtlInDays));

            RefreshTokens.Add(refreshToken);
            RefreshTokens.RemoveAll(x => !x.IsActive && x.Created.AddDays(refreshTokenTtlInDays) <= dateTimeNow);
        }
    }
}