using Kursach.Model.Repositories;

namespace Kursach.Model.Models;

public class Supplier : ISupplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SupplyHistory { get; set; } = string.Empty;
}