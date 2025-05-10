using Kursach.Model.Repositories;

namespace Kursach.Model.Models;

public class WarrantyService : IWarrantyService
{
    public int Id;
    public string ProductId  ;
    public string ClientId  ;
    public string IssueDescription  ;
    public string AcceptanceDate  ;
    public string CompletionDate    ;
    public string Status   ;
}