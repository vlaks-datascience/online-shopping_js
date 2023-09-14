using System;

namespace back_shopping.DTOs
{
    public class OrderProductDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public Nullable<double> ProductPrice { get; set; }
    }
}
