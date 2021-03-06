using CSharpFunctionalExtensions;
using Domain.Products.Entities;
using Domain.Shared;
using Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Orders.Entities
{
    public class Order : BaseEntity
    {
        public Email Email { get; }
        public virtual Name Name { get; }
        public virtual PhoneNumber PhoneNumber { get; }

        public Price FullPrice { get; private set; }
        public int OrderItemsCount { get; private set; }
        public OrderState OrderState { get; private set; }

        private readonly List<OrderItem> _orderItems;
        public virtual IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();


        protected Order() { }
        private Order(Email email, PhoneNumber phoneNumber, Name name)
        {
            Email = email;
            Name = name;
            PhoneNumber = phoneNumber;
            _orderItems = new List<OrderItem>();
            FullPrice = Price.Empty;
            OrderItemsCount = 0;
            OrderState = OrderState.PaymentNew;
        }

        public static Result<Order> Create(Email email, PhoneNumber phoneNumber, Name name)
        {
            if (email == null)
                return Result.Failure<Order>("Email cannot be empty");
            if (name == null)
                return Result.Failure<Order>("Name cannot be empty");
            if (phoneNumber == null)
                return Result.Failure<Order>("Email cannot be empty");

            return Result.Success(new Order(email, phoneNumber, name));
        }


        public Result AddOrderItem(Product product, int quantity)
        {
            if (product == null)
                return Result.Failure("Invalid product");
            if (quantity < 1)
                return Result.Failure("Invalid product quantity");
            if (quantity > product.Quantity)
                return Result.Failure("Invalid product quantity");

            var decreaseProductQuantityResult = product.DecreaseQuantity(quantity);
            if (decreaseProductQuantityResult.IsFailure)
                return decreaseProductQuantityResult;

            var existingOrderItem = _orderItems.FirstOrDefault(x => x.Product == product);
            if (existingOrderItem == null)
                _orderItems.Add(new OrderItem(product, this, quantity));
            else
                existingOrderItem.AddQuantity(quantity);

            FullPrice += product.Price * quantity;
            OrderItemsCount += quantity;

            return Result.Success();
        }

        public Result RemoveOrderItem(OrderItem orderItem, int quantity, DateTime deleteDate)
        {
            if (orderItem == null)
                return Result.Failure("Invalid order item");
            if (quantity < 1 || quantity > orderItem.Quantity)
                return Result.Failure("Invalid quantity");

            if (orderItem.Quantity == quantity)
                orderItem.Delete(deleteDate);

            orderItem.Product.IncreaseQuantity(quantity);
            FullPrice -= orderItem.Product.Price * quantity;
            OrderItemsCount -= quantity;

            return Result.Success();
        }

        public void SetOrderSubmitted(DateTime paymentStartDate)
        {
            OrderState = OrderState.OrderSubmitted(OrderState, paymentStartDate);
        }

        public void SetPaymentInProgress(DateTime paymentInProgressDate)
        {
            OrderState = OrderState.PaymentInProgress(OrderState, paymentInProgressDate);
        }

        public void SetPaymentError(DateTime paymentErrorDate)
        {
            OrderState = OrderState.PaymentError(OrderState, paymentErrorDate);
        }
    }
}
