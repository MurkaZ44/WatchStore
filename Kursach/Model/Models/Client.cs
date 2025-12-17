using Kursach.Model.Repositories;

namespace Kursach.Model.Models;

public class Client : IClient
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PurchaseHistory { get; set; } = string.Empty;
}