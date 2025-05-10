using Kursach.Model.Repositories;

namespace Kursach.Model.Models;

public class Product : IProduct
{
    public int Id;
    public string _type;
    public string Model;
    public string Brand;
    public string SerialNumber ;
    public float Price;
    public int QuantityInStock;
    public int WarrantyPeriod;
}