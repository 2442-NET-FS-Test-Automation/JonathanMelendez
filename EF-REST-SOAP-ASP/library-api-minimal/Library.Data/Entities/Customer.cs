using System.ComponentModel.DataAnnotations;

namespace Library.Data.Entities;

public class Customer()
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public List<Order> Orders { get; set; } = [];
}