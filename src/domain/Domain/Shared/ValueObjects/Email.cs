using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Domain.Shared.ValueObjects
{
    public class Email : ValueObject
    {
        public string Value { get; }


        protected Email() { }
        private Email(string email)
        {
            Value = email;
        }

        public static Result<Email> Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure<Email>("Email should not be empty");

            email = email.Trim();
            if (email.Length > 100)
                return Result.Failure<Email>("Email is too long");
            if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
                return Result.Failure<Email>("Email is invalid");

            return Result.Success(new Email(email));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(Email email)
        {
            return email.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
