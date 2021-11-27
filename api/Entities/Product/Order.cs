using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities.Shared;

namespace WebApi.Entities.Product
{
    public class Order : BaseEntity
    {
        public Email Email { get; }
        public virtual Name Name { get; }
        public virtual PhoneNumber PhoneNumber { get; }

        private readonly List<OrderItem> _orderItems;
        public virtual IReadOnlyList<OrderItem> OrderItems => _orderItems.ToList();


        protected Order() { }
        private Order(Email email, PhoneNumber phoneNumber, Name name, DateTime createDate) : base(createDate)
        {
            Email = email;
            Name = name;
            PhoneNumber = phoneNumber;
            _orderItems = new List<OrderItem>();
        }

        public static Result<Order> Create(Email email, PhoneNumber phoneNumber, Name name, DateTime createDate)
        {
            if(createDate == default)
                return Result.Failure<Order>("Create date cannot be empty");
            if (email == null)
                return Result.Failure<Order>("Email cannot be empty");
            if (name == null)
                return Result.Failure<Order>("Name cannot be empty");
            if (phoneNumber == null)
                return Result.Failure<Order>("Email cannot be empty");

            return Result.Success(new Order(email, phoneNumber, name, createDate));
        }


        public Result AddOrderItem(Product product, int quantity, DateTime createDate)
        {
            if (product == null)
                return Result.Failure("Invalid product");
            if (quantity < 1)
                return Result.Failure("Invalid product quantity");

            _orderItems.Add(new OrderItem(product, this, quantity, createDate));
            return Result.Success();
        }
    }

    public class OrderItem : BaseEntity
    {
        public virtual Product Product { get; private set; }
        public virtual Order Order { get; private set; }
        public int Quantity {  get; set; }

        protected OrderItem() { }
        public OrderItem(Product product, Order order, int quantity, DateTime createDate) : base(createDate)
        {
            Product = product;
            Order = order;
            Quantity = quantity;
        }
    }
}
