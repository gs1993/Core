using CSharpFunctionalExtensions;
using System;
using WebApi.Entities.Shared;

namespace WebApi.Entities.Product
{
    public class Product : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Quantity { get; private set; }
        public Price Price { get; private set; }

        protected Product() { }
        private Product(string name, string description, int quantity, Price price, DateTime createDate) : base(createDate)
        {
            Name = name;
            Description = description;
            Quantity = quantity;
            Price = price;
        }

        public static Result<Product> Create(string name, string description, int quantity, Price price, DateTime createDate)
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

            return Result.Success(new Product(name, description, quantity, price, createDate));
        }

        

    }
}
