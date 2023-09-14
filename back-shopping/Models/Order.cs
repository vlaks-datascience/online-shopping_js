using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_shopping.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Price { get; set; }
        public DateTime OrderDate { get; set; }
        public string Comment { get; set; }
        public string Address { get; set; }
        public DateTime OrderExpire { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsSent { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderProduct> OrderProducts { get; set; }
    }
}
