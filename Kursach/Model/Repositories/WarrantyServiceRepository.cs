using Kursach.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Model.Repositories;

public class WarrantyServiceRepository
{
    private readonly AppDbContext _context;

    public WarrantyServiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WarrantyService>> GetAllAsync()
    {
        return await _context.WarrantyServices
            .Include(w => w.Product)
            .Include(w => w.Client)
            .ToListAsync();
    }

    public async Task<WarrantyService?> GetByIdAsync(int id)
    {
        return await _context.WarrantyServices
            .Include(w => w.Product)
            .Include(w => w.Client)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<WarrantyService> AddAsync(WarrantyService warrantyService)
    {
        _context.WarrantyServices.Add(warrantyService);
        await _context.SaveChangesAsync();
        return warrantyService;
    }

    public async Task<WarrantyService> UpdateAsync(WarrantyService warrantyService)
    {
        _context.WarrantyServices.Update(warrantyService);
        await _context.SaveChangesAsync();
        return warrantyService;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var warrantyService = await _context.WarrantyServices.FindAsync(id);
        if (warrantyService == null) return false;
        
        _context.WarrantyServices.Remove(warrantyService);
        await _context.SaveChangesAsync();
        return true;
    }
}