using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Shared.ValueObjects
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
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return Result.Failure<PhoneNumber>("Phone number cannot be empty");
            var phoneNumberDigits = new string(phoneNumber.Where(char.IsDigit).ToArray());
            if (phoneNumberDigits.Length < 6)
                return Result.Failure<PhoneNumber>("Phone number too short");
            if (phoneNumberDigits.Length > 15)
                return Result.Failure<PhoneNumber>("Phone number too long");

            if (string.IsNullOrWhiteSpace(countryCallingCode))
                return Result.Failure<PhoneNumber>("Country calling code cannot be empty");
            var countryCallingCodeDigits = new string(countryCallingCode.Where(char.IsDigit).ToArray());
            if (countryCallingCodeDigits.Length < 2)
                return Result.Failure<PhoneNumber>("Country calling code number too short");
            if (countryCallingCodeDigits.Length > 5)
                return Result.Failure<PhoneNumber>("Country calling code number too long");

            return Result.Success(new PhoneNumber(phoneNumberDigits, countryCallingCode));
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
