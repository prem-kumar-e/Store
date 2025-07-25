using Microsoft.EntityFrameworkCore;
using Store.Models;

namespace Store.Data
{
    public class InvoiceService
    {
        private readonly StoreDBContext _context;

        public InvoiceService(StoreDBContext context)
        {
            _context = context;
        }
        public async Task<List<Invoice>> GetInvoicesAsync()
        {
            return await _context.Invoices.Include(i=>i.Items).ThenInclude(item => item.Product).ToListAsync();
        }
        public async Task<Invoice?> GetInvoiceAsync(string BillNo)
        {
            return await _context.Invoices.Where(i => i.BillNo == BillNo).Include(i => i.Items).FirstOrDefaultAsync();
        }
        public async Task AddInvoiceAsync(Invoice invoice)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await SaveInvoiceAndUpdateStockAsync(invoice);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task SaveInvoiceAndUpdateStockAsync(Invoice invoice)
        {
            // Attach and save the invoice
            _context.Invoices.Add(invoice);

            // Loop through each item and deduct stock
            foreach (var item in invoice.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    if (product.StockQuantity < item.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product: {product.Id}");
                    }

                    product.StockQuantity -= item.Quantity;

                    // Optional: update timestamp or log the stock change
                    _context.Products.Update(product);
                }
            }

            await _context.SaveChangesAsync();
        }
        public async Task UpdateInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteInvoiceAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }
        public string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}";
        }
        public decimal GetTax()
        {
            return _context.AdminSettings.Select(x => x.Tax).FirstOrDefault();
        }
    }
}
