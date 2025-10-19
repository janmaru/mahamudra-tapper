namespace Mahamudra.Tapper.Tests.Categories;

/// <summary>
/// Category entity - represents a product category in the domain
/// </summary>
public sealed class Category
{
    private const int MaxNameLength = 255;
    private const int MinNameLength = 1;

    // Private constructor for EF Core
    private Category() { }

    // Factory method for creating new categories
    public static Category Create(string name)
    {
        ValidateName(name);

        return new Category
        {
            Name = name.Trim()
        };
    }

    // Factory method for reconstituting from database
    internal static Category Reconstitute(int id, string name)
    {
        return new Category
        {
            Id = id,
            Name = name
        };
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    public void UpdateName(string newName)
    {
        ValidateName(newName);
        Name = newName.Trim();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty or whitespace.", nameof(name));

        if (name.Trim().Length < MinNameLength)
            throw new ArgumentException($"Category name must be at least {MinNameLength} character.", nameof(name));

        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Category name cannot exceed {MaxNameLength} characters.", nameof(name));
    }
} 