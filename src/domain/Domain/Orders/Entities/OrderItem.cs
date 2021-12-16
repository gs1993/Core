using Domain.Products.Entities;
using Domain.Shared;
using System;

namespace Domain.Orders.Entities
{
    public class OrderItem : BaseEntity
    {
        public virtual Product Product { get; private set; }
        public virtual Order Order { get; private set; }
        public int Quantity { get; private set; }

        protected OrderItem() { }
        public OrderItem(Product product, Order order, int quantity)
        {
            Product = product;
            Order = order;
            Quantity = quantity;
        }

        public void AddQuantity(int quantityToAdd)
        {
            if (quantityToAdd < 0)
                throw new ArgumentOutOfRangeException(nameof(quantityToAdd));

            Quantity += quantityToAdd;
        }
    }
}
