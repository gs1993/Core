using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Entities.Site
{
    public class Address : ValueObject
    {
        public string FirstLine { get; }
        public string SecondLine { get; }

        protected Address() { }
        private Address(string firstLine, string secondLine)
        {
            FirstLine = firstLine;
            SecondLine = secondLine;
        }

        public static Result<Address> Create(string firstLine, string secondLine)
        {
            if (string.IsNullOrWhiteSpace(firstLine))
                return Result.Failure<Address>("Address first line cannot be empty");
            if (string.IsNullOrWhiteSpace(secondLine))
                return Result.Failure<Address>("Address second line cannot be empty");

            return Result.Success(new Address(firstLine, secondLine));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstLine;
            yield return SecondLine;
        }
    }
}
