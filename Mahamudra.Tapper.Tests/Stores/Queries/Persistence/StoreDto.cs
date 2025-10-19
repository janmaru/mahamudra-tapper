using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mahamudra.Tapper.Tests.Stores.Queries.Persistence;

/// <summary>
/// Data Transfer Object for Store - used only in infrastructure layer for database mapping
/// </summary>
[Table("stores")]
internal sealed class StoreDto
{
    [Column("store_id")]
    [Required]
    public Guid Id { get; set; }

    [Column("store_name")]
    [Required(AllowEmptyStrings = false)]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Column("phone")]
    [StringLength(25)]
    public string? Phone { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Column("street")]
    [StringLength(255)]
    public string? Street { get; set; }

    [Column("city")]
    [StringLength(255)]
    public string? City { get; set; }

    [Column("state")]
    [StringLength(10)]
    public string? State { get; set; }

    [Column("zip_code")]
    [StringLength(5)]
    public string? ZipCode { get; set; }

    public Store ToDomain()
    {
        return Store.Reconstitute(Id, Name, Phone, Email, Street, City, State, ZipCode);
    }
}
