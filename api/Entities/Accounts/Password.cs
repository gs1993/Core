using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;

namespace WebApi.Entities.Accounts
{
    public class Password : ValueObject
    {
        public string PasswordHash { get; }
        public string PasswordSalt { get; }


        protected Password() { }
        private Password(string passwordHash, string passwordSalt)
        {
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
        }

        public static Result<Password> Create(string passwordHash, string passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(passwordHash) || string.IsNullOrWhiteSpace(passwordSalt))
                return Result.Failure<Password>("Invalid password.");

            return Result.Success(new Password(passwordHash, passwordSalt));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            throw new NotImplementedException();
        }
    }
}
