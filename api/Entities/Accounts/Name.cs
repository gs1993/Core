using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace WebApi.Entities.Accounts
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
                return Result.Failure<Name>("'LastName' cannot be empty.");
            if (string.IsNullOrWhiteSpace(firstName))
                return Result.Failure<Name>("'FirstName' cannot be empty.");

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
