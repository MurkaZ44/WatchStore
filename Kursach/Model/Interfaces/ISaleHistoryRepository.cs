using System.Collections.Generic;
using Kursach.Model.Models;

namespace Kursach.Model.Interfaces
{
    public interface ISaleHistoryRepository
    {
        IEnumerable<Sale> GetSalesHistoryForProduct(int productId);
    }
}