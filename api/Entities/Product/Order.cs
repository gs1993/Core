using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities.Shared;

namespace WebApi.Entities.Product
{
    public class Order : BaseEntity
    {
        public virtual BuyerData BuyerData { get; private set; }

        private readonly List<OrderItem> _orderItems;
        public virtual IReadOnlyList<OrderItem> OrderItems => _orderItems.ToList();


        protected Order() { }
        private Order(BuyerData buyerData)
        {
            BuyerData = buyerData;
            _orderItems = new List<OrderItem>();
        }

        public static Result<Order> Create(BuyerData buyerData)
        {
            if (buyerData == null)
                return Result.Failure<Order>("Buyer data cannot be empty");

            return Result.Success(new Order(buyerData));
        }


        public Result AddOrderItem(Product product, int quantity)
        {
            if (product == null)
                return Result.Failure("Invalid product");
            if (quantity < 1)
                return Result.Failure("Invalid product quantity");

            _orderItems.Add(new OrderItem(product, quantity));
            return Result.Success();
        }
    }

    public class OrderItem : BaseEntity
    {
        public Product Product { get; set; }
        public int Quantity {  get; set; }

        protected OrderItem() { }
        public OrderItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        //public static Result<OrderItem>(Create)
    }
}
