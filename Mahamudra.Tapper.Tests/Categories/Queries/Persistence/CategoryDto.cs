using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mahamudra.Tapper.Tests.Categories.Queries.Persistence;

/// <summary>
/// Data Transfer Object for Category - used only in infrastructure layer for database mapping
/// </summary>
[Table("categories")]
internal sealed class CategoryDto
{
    [Column("category_id")]
    [Required]
    public int Id { get; set; }

    [Column("category_name")]
    [Required(AllowEmptyStrings = false)]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    public Category ToDomain()
    {
        return Category.Reconstitute(Id, Name);
    }
}
