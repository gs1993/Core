using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace Domain.Shared.ValueObjects
{
    public class Name : ValueObject
    {
        public string Title { get; }
        public string FirstName { get; }
        public string LastName { get; }

        protected Name() { }
        private Name(string title, string firstName, string lastName)
        {
            Title = title;
            FirstName = firstName;
            LastName = lastName;
        }

        public static Result<Name> Create(string title, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
                return Result.Failure<Name>("Last name cannot be empty");
            if (lastName.Length > 50)
                return Result.Failure<Name>("Last name too long");
            if (string.IsNullOrWhiteSpace(firstName))
                return Result.Failure<Name>("First name cannot be empty");
            if (firstName.Length > 50)
                return Result.Failure<Name>("First name too long");
            if (title?.Length > 5)
                return Result.Failure<Name>("Title too long");

            string nameTitle = string.IsNullOrWhiteSpace(title) ? string.Empty : title;
            return Result.Success(new Name(nameTitle, firstName, lastName));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return LastName;
            yield return FirstName;
            yield return Title;
        }
    }
}
