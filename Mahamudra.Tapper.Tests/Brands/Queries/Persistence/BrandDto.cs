using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mahamudra.Tapper.Tests.Brands.Queries.Persistence;

/// <summary>
/// Data Transfer Object for Brand - used only in infrastructure layer for database mapping
/// </summary>
[Table("brands")]
internal sealed class BrandDto
{
    [Column("brand_id")]
    [Required]
    public int Id { get; set; }

    [Column("brand_name")]
    [Required(AllowEmptyStrings = false)]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    public Brand ToDomain()
    {
        return Brand.Reconstitute(Id, Name);
    }
}
