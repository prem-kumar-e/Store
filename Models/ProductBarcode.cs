using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class ProductBarcode
    {
        public int Id { get; set; }
        [Required]
        public string Barcode {  get; set; }
        public int? InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}
