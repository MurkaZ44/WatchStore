using Kursach.Model.Models;
using Microsoft.EntityFrameworkCore;
namespace Kursach.Model.Repositories;

public class SaleRepository
{
    private readonly AppDbContext _context;

    public SaleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Sale>> GetAllAsync()
    {
        return await _context.Sales
            .Include(s => s.Product)
            .Include(s => s.Client)
            .Include(s => s.Seller)
            .ToListAsync();
    }

    public async Task<Sale?> GetByIdAsync(int id)
    {
        return await _context.Sales
            .Include(s => s.Product)
            .Include(s => s.Client)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Sale> AddAsync(Sale sale)
    {
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();
        return sale;
    }

    public async Task<Sale> UpdateAsync(Sale sale)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync();
        return sale;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale == null) return false;
        
        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync();
        return true;
    }
}