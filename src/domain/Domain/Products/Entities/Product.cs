using CSharpFunctionalExtensions;
using Domain.Shared;
using System;

namespace Domain.Products.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Quantity { get; private set; }
        public Price Price { get; private set; }

        protected Product() { }
        private Product(string name, string description, int quantity, Price price)
        {
            Name = name;
            Description = description;
            Quantity = quantity;
            Price = price;
        }

        public static Result<Product> Create(string name, string description, int quantity, Price price)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<Product>("Name cannot be empty");
            if (name.Length > 150)
                return Result.Failure<Product>("Name is too long");
            if (string.IsNullOrWhiteSpace(description))
                return Result.Failure<Product>("Description cannot be empty");
            if (description.Length > 500)
                return Result.Failure<Product>("Description is too long");
            if (quantity < 0)
                return Result.Failure<Product>("Invalid quantity");
            if (price == null)
                return Result.Failure<Product>("Invalid price");

            return Result.Success(new Product(name, description, quantity, price));
        }


        public Result IncreaseQuantity(int quantityToIncrease)
        {
            if (quantityToIncrease < 0)
                throw new ArgumentOutOfRangeException(nameof(quantityToIncrease));

            Quantity += quantityToIncrease;
            return Result.Success();
        }

        public Result DecreaseQuantity(int quantityToDecrease)
        {
            if (quantityToDecrease < 0)
                throw new ArgumentOutOfRangeException(nameof(quantityToDecrease));
            if (quantityToDecrease > Quantity)
                return Result.Failure("Not enougth product quantity");

            Quantity -= quantityToDecrease;
            return Result.Success();
        }
    }
}
