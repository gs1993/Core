using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace WebApi.Entities.Product
{
    public class Price : ValueObject
    {
        public decimal Value { get; }
        public string Currency { get; }

        protected Price() { }
        private Price(decimal value, string currency)
        {
            Value = value;
            Currency = currency.ToUpper();
        }

        public static Result<Price> Create(decimal value, string currency)
        {
            if (value < 0)
                return Result.Failure<Price>("Invalid price");
            if (string.IsNullOrWhiteSpace(currency))
                return Result.Failure<Price>("Currency cannot be empty");
            if(currency.Length != 3)
                return Result.Failure<Price>("Invalid currency");

            return Result.Success(new Price(value, currency));
        }


        public override string ToString()
        {
            return $"{Value:0.00} {Currency}";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Currency;
        }
    }
}
