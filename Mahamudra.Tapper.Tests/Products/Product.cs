using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mahamudra.Tapper.Tests.Products;

[Table("products")]
public class Product
{
    [Column("product_id")]
    [Required]
    public int Id { get; set; }

    [Column("product_name")]
    [Required(AllowEmptyStrings = false)]
    public string? Name { get; set; }

    [Column("brand_id")]
    [Required]
    public int BrandId { get; set; }

    [Column("category_id")]
    [Required]
    public int CategoryId { get; set; }

    [Column("model_year")]
    [Required]
    public Int16 ModelYear { get; set; }

    [Column("list_price")]
    [Required]
    public decimal ListPrice { get; set; } 
    public Category? Category { get; set; }
} 