using Team6.Domain.Catalog;

namespace Team6.Domain.Orders
{
    public class OrderItem
    {
        public int Id{get; set;}
        public Item Item{get; set;}
        public int Quantity{get; set;}
        public decimal Price => Item.Price * Quantity;
    }
}