using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace WebApi.Entities.Shared
{
    public class PhoneNumber : ValueObject
    {
        public string Value { get; }
        public string CountryCallingCode { get; }


        protected PhoneNumber() { }
        private PhoneNumber(string value, string countryCallingCode)
        {
            Value = value;
            CountryCallingCode = countryCallingCode;
        }

        public static Result<PhoneNumber> Create(string phoneNumber, string countryCallingCode)
        {
            if (!string.IsNullOrWhiteSpace(phoneNumber))
                return Result.Failure<PhoneNumber>("Phone number cannot be empty");
            if (phoneNumber.Length > 15)
                return Result.Failure<PhoneNumber>("Phone number too long");

            if(!string.IsNullOrWhiteSpace(countryCallingCode))
                return Result.Failure<PhoneNumber>("Country calling code cannot be empty");
            countryCallingCode = countryCallingCode.Replace("+", "");
            if (countryCallingCode.Length > 6)
                return Result.Failure<PhoneNumber>("Country calling code number too long");

            return Result.Success(new PhoneNumber(phoneNumber, countryCallingCode));
        }

        public override string ToString()
        {
            return $"+{CountryCallingCode} {Value}";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return CountryCallingCode;
        }
    }
}
