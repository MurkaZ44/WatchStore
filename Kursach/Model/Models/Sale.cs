using Kursach.Model.Repositories;

namespace Kursach.Model.Models;

public class Sale : ISale
{
    public int Id;
    public string ProductId ;
    public string ClientId ;
    public string Date ;
    public string Price ;
    public string PaymentType ;
    public string Seller ;
}