using back_shopping.Models;
using System;
using System.Collections.Generic;

namespace back_shopping.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Price { get; set; }
        public DateTime OrderDate { get; set; }
        public string Comment { get; set; }
        public string Address { get; set; }
        public List<OrderProduct> OrderProducts { get; set; }
        public DateTime OrderExpire { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsSent { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentMethod { get; set; }
    }
}
