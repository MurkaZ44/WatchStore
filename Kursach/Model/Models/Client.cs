using Kursach.Model.Repositories;

namespace Kursach.Model.Models;

public class Client : IClient
{
    public int Id;
    public string FullName;
    public string Phone;
    public string Email;
    public string PurchaseHistory;
}