using Kursach.Model.Repositories;

namespace Kursach.Model.Models;

public class WarrantyService : IWarrantyService
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int ClientId { get; set; }
    public string IssueDescription { get; set; } = string.Empty;
    public DateTime AcceptanceDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string Status { get; set; } = string.Empty;
    
    // Навигационные свойства для EF Core
    public Product? Product { get; set; }
    public Client? Client { get; set; }
}