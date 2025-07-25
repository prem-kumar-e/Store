namespace Store.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }

        // Navigation property
        public Category? Category { get; set; }
    }

    public class ProductUnit
    {
        public int Id { get; set; }
        public string Barcode { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public bool IsSold { get; set; } = false;
        public DateTime? SoldOn { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public required string Name { get; set; }

        public ICollection<Product>? Products { get; set; }
    }


}
