using System.ComponentModel.DataAnnotations;

namespace Kursach.Model.Models;

public class Seller
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
}