using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mahamudra.Tapper.Tests.Products;

[Table("brands")]
public class Brand
{
    [Column("brand_id")]
    [Required]
    public int Id { get; set; }

    [Column("brand_name")]
    [Required(AllowEmptyStrings = false)]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Category name must be from 1 to 255 characters!")]
    public string? Name { get; set; }
} 