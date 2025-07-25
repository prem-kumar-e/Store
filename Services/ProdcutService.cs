using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;

using Store.Models;
using System;

namespace Store.Data
{
    public class ProductService
    {
        private readonly StoreDBContext _context;

        public ProductService(StoreDBContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product;
        }

        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            try
            {
                var existing = await _context.Products.FindAsync(product.Id);
                if (existing != null)
                {
                    existing.Name = product.Name;
                    existing.Price = product.Price;
                    existing.StockQuantity = product.StockQuantity;
                    existing.CategoryId = product.CategoryId;
                    await _context.SaveChangesAsync();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddProductBarcode(List<ProductBarcode> barcodes)
        {
            try {
                var inputBarcodes = barcodes.Select(b => b.Barcode).ToList();
                var existingBarcodes = await _context.Barcodes
               .Where(b => inputBarcodes.Contains(b.Barcode))
               .Select(b => b.Barcode)
               .ToListAsync();

                if (existingBarcodes.Any())
                    throw new Exception($"Barcodes already exist in DB: {string.Join(", ", existingBarcodes)}");

                await _context.Barcodes.AddRangeAsync(barcodes);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or return a message indicating barcode already exists
                throw new Exception("Duplicate barcode detected", ex);
            }
        }

        public async Task UpdateProductBarcode(List<string> barcodes, Invoice invoice)
        {
            try
            {
                _context.Barcodes
                    .Where(p => barcodes.Contains(p.Barcode))
                    .ToList()
                    .ForEach(pb =>
                    {
                        pb.Invoice = invoice;
                        pb.InvoiceId = invoice.Id;
                    });
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or return a message indicating barcode already exists
                throw new Exception("Duplicate barcode detected", ex);
            }
        }
        public async Task<ProductBarcode> GetBarcodeProduct(string barcode)
        {
            var product = await _context.Barcodes
                .Where(b => barcode.Contains(b.Barcode))
                .FirstOrDefaultAsync();
            return product == null
                    ? throw new Exception("Invalid Barcode")
                    : product.InvoiceId == null
                        ? product
                        : throw new Exception("Invalid Item");
        }
    }

    public class CategoryService
    {
        private readonly StoreDBContext _context;

        public CategoryService(StoreDBContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<Category> GetProductAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            return category;
        }
        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }

}
