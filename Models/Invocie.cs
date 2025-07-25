using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Store.Models
{
    public class Invoice
    {
        public string? CusName { get; set; }
        public int Id { get; set; }
        public string BillNo { get; set; }
        public DateTime Date { get; set; }
        public List<InvoiceItem> Items{ get; set; }
        public decimal Amount { get; set; }
        public decimal? Tax { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? TotalAmount { get; set; }
    }

    public class InvoiceItem
    {
        public int Id { get; set; } // Primary key
        public int InvoiceId { get; set; }

        [JsonIgnore]
        public Invoice Invoice { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }

    }
    public class UserModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Age { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public DateOnly DoC { get; set; }
    }
}
