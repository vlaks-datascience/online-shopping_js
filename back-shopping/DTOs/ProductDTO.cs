namespace back_shopping.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public byte[] Image { get; set; }
        public int UserId { get; set; }
    }
}
