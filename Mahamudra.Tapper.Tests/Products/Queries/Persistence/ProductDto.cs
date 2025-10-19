using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mahamudra.Tapper.Tests.Categories;

namespace Mahamudra.Tapper.Tests.Products.Queries.Persistence;

/// <summary>
/// Data Transfer Object for Product - used only in infrastructure layer for database mapping
/// </summary>
[Table("products")]
internal sealed class ProductDto
{
    [Column("product_id")]
    [Required]
    public int Id { get; set; }

    [Column("product_name")]
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;

    [Column("brand_id")]
    [Required]
    public int BrandId { get; set; }

    [Column("category_id")]
    [Required]
    public int CategoryId { get; set; }

    [Column("model_year")]
    [Required]
    public short ModelYear { get; set; }

    [Column("list_price")]
    [Required]
    public decimal ListPrice { get; set; }

    public Category? Category { get; set; }

    public Product ToDomain()
    {
        return Product.Reconstitute(Id, Name, BrandId, CategoryId, ModelYear, ListPrice, Category);
    }
}
