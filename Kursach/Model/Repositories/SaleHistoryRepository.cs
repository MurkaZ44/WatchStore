using System.Collections.Generic;
using System.Linq;
using Kursach.Model.Interfaces;
using Kursach.Model.Models;
using Kursach; // For AppDbContext

namespace Kursach.Model.Repositories
{
    public class SaleHistoryRepository : ISaleHistoryRepository
    {
        private readonly AppDbContext _context;

        public SaleHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Sale> GetSalesHistoryForProduct(int productId)
        {
            // For now, return a dummy list or query if a DbSet exists.
            // Assuming AppDbContext has a DbSet<Sale> named Sales.
            // If not, this would need adjustment.
            return _context.Sales.Where(s => s.ProductId == productId).ToList();
        }
    }
}