namespace Mahamudra.Tapper.Tests.Brands;

/// <summary>
/// Brand entity - represents a product brand in the domain
/// </summary>
public sealed class Brand
{
    private const int MaxNameLength = 255;
    private const int MinNameLength = 1;

    // Private constructor for EF Core
    private Brand() { }

    // Factory method for creating new brands
    public static Brand Create(string name)
    {
        ValidateName(name);

        return new Brand
        {
            Name = name.Trim()
        };
    }

    // Factory method for reconstituting from database
    internal static Brand Reconstitute(int id, string name)
    {
        return new Brand
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
            throw new ArgumentException("Brand name cannot be empty or whitespace.", nameof(name));

        if (name.Trim().Length < MinNameLength)
            throw new ArgumentException($"Brand name must be at least {MinNameLength} character.", nameof(name));

        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Brand name cannot exceed {MaxNameLength} characters.", nameof(name));
    }
} 