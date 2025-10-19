using Mahamudra.Tapper.Tests.Categories;

namespace Mahamudra.Tapper.Tests.Products;

/// <summary>
/// Product entity - represents a product in the domain
/// </summary>
public sealed class Product
{
    private const int MaxNameLength = 255;
    private const int MinNameLength = 1;

    // Private constructor for EF Core
    private Product() { }

    // Factory method for creating new products
    public static Product Create(
        string name,
        int brandId,
        int categoryId,
        short modelYear,
        decimal listPrice)
    {
        ValidateName(name);
        ValidateBrandId(brandId);
        ValidateCategoryId(categoryId);
        ValidateModelYear(modelYear);
        ValidateListPrice(listPrice);

        return new Product
        {
            Name = name.Trim(),
            BrandId = brandId,
            CategoryId = categoryId,
            ModelYear = modelYear,
            ListPrice = listPrice
        };
    }

    // Factory method for reconstituting from database
    internal static Product Reconstitute(
        int id,
        string name,
        int brandId,
        int categoryId,
        short modelYear,
        decimal listPrice,
        Category? category = null)
    {
        return new Product
        {
            Id = id,
            Name = name,
            BrandId = brandId,
            CategoryId = categoryId,
            ModelYear = modelYear,
            ListPrice = listPrice,
            Category = category
        };
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int BrandId { get; private set; }
    public int CategoryId { get; private set; }
    public short ModelYear { get; private set; }
    public decimal ListPrice { get; private set; }
    public Category? Category { get; private set; }

    public void UpdateName(string newName)
    {
        ValidateName(newName);
        Name = newName.Trim();
    }

    public void UpdatePrice(decimal newPrice)
    {
        ValidateListPrice(newPrice);
        ListPrice = newPrice;
    }

    public void UpdateModelYear(short newModelYear)
    {
        ValidateModelYear(newModelYear);
        ModelYear = newModelYear;
    }

    public void AssignCategory(int categoryId)
    {
        ValidateCategoryId(categoryId);
        CategoryId = categoryId;
    }

    public void AssignBrand(int brandId)
    {
        ValidateBrandId(brandId);
        BrandId = brandId;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty or whitespace.", nameof(name));

        if (name.Trim().Length < MinNameLength)
            throw new ArgumentException($"Product name must be at least {MinNameLength} character.", nameof(name));

        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Product name cannot exceed {MaxNameLength} characters.", nameof(name));
    }

    private static void ValidateBrandId(int brandId)
    {
        if (brandId <= 0)
            throw new ArgumentException("Brand ID must be greater than zero.", nameof(brandId));
    }

    private static void ValidateCategoryId(int categoryId)
    {
        if (categoryId <= 0)
            throw new ArgumentException("Category ID must be greater than zero.", nameof(categoryId));
    }

    private static void ValidateModelYear(short modelYear)
    {
        if (modelYear < 1900 || modelYear > 9999)
            throw new ArgumentException("Model year must be between 1900 and 9999.", nameof(modelYear));
    }

    private static void ValidateListPrice(decimal listPrice)
    {
        if (listPrice < 0)
            throw new ArgumentException("List price cannot be negative.", nameof(listPrice));
    }
} 