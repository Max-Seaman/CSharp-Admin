using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSharp_Admin.Models;

public class Company
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    [NotMapped]
    public IFormFile? Logo { get; set; }

    public string? LogoPath { get; set; }

    [Url]
    [StringLength(100)]
    public string? Website { get; set; }

    public List<Employee> Employees { get; set; } = new();
}
