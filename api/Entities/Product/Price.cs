using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;

namespace WebApi.Entities.Product
{
    public class Price : ValueObject
    {
        public static Price Empty = new(0, string.Empty);

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

        public static Price operator +(Price a, Price b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));
            if (a == Empty)
                return b;
            if (b == Empty)
                return a;

            if (a.Currency != b.Currency)
                throw new ArgumentException($"Cannot add different currencies");

            return new Price(a.Value + b.Value, a.Currency);
        }

        public static Price operator -(Price a, Price b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));
            if (a == Empty)
                throw new ArgumentException("Invalid value");
            if (b == Empty)
                return a;

            if (a.Currency != b.Currency)
                throw new ArgumentException("Cannot subtract different currencies");
            decimal newValue = a.Value - b.Value;
            if (newValue < 0)
                throw new ArgumentException("Invalid value");

            return new Price(a.Value - b.Value, a.Currency);
        }

        public static Price operator *(Price a, int b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b <= 0)
                throw new ArgumentNullException(nameof(b));

            return new Price(a.Value * b, a.Currency);
        }
    }
}
