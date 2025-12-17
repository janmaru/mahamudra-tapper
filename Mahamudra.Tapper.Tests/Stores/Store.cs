namespace Mahamudra.Tapper.Tests.Stores;

/// <summary>
/// Store entity - represents a store location in the domain
/// </summary>
public sealed class Store
{
    private const int MaxNameLength = 255;
    private const int MinNameLength = 1;
    private const int MaxPhoneLength = 25;
    private const int MaxEmailLength = 255;
    private const int MaxStreetLength = 255;
    private const int MaxCityLength = 255;
    private const int MaxStateLength = 10;
    private const int MaxZipCodeLength = 5;

    // Private constructor for EF Core
    private Store() { }

    // Factory method for creating new stores
    public static Store Create(
        string name,
        string? phone = null,
        string? email = null,
        string? street = null,
        string? city = null,
        string? state = null,
        string? zipCode = null)
    {
        ValidateName(name);
        ValidatePhone(phone);
        ValidateEmail(email);
        ValidateStreet(street);
        ValidateCity(city);
        ValidateState(state);
        ValidateZipCode(zipCode);

        return new Store
        {
            Name = name.Trim(),
            Phone = phone?.Trim(),
            Email = email?.Trim(),
            Street = street?.Trim(),
            City = city?.Trim(),
            State = state?.Trim(),
            ZipCode = zipCode?.Trim()
        };
    }

    // Factory method for reconstituting from database
    internal static Store Reconstitute(
        int id,
        string name,
        string? phone = null,
        string? email = null,
        string? street = null,
        string? city = null,
        string? state = null,
        string? zipCode = null)
    {
        return new Store
        {
            Id = id,
            Name = name,
            Phone = phone,
            Email = email,
            Street = street,
            City = city,
            State = state,
            ZipCode = zipCode
        };
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Street { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? ZipCode { get; private set; }

    public void UpdateName(string newName)
    {
        ValidateName(newName);
        Name = newName.Trim();
    }

    public void UpdateContactInfo(string? phone = null, string? email = null)
    {
        if (phone != null)
        {
            ValidatePhone(phone);
            Phone = phone.Trim();
        }

        if (email != null)
        {
            ValidateEmail(email);
            Email = email.Trim();
        }
    }

    public void UpdateAddress(string? street = null, string? city = null, string? state = null, string? zipCode = null)
    {
        if (street != null)
        {
            ValidateStreet(street);
            Street = street.Trim();
        }

        if (city != null)
        {
            ValidateCity(city);
            City = city.Trim();
        }

        if (state != null)
        {
            ValidateState(state);
            State = state.Trim();
        }

        if (zipCode != null)
        {
            ValidateZipCode(zipCode);
            ZipCode = zipCode.Trim();
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Store name cannot be empty or whitespace.", nameof(name));

        if (name.Trim().Length < MinNameLength)
            throw new ArgumentException($"Store name must be at least {MinNameLength} character.", nameof(name));

        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Store name cannot exceed {MaxNameLength} characters.", nameof(name));
    }

    private static void ValidatePhone(string? phone)
    {
        if (phone != null && phone.Length > MaxPhoneLength)
            throw new ArgumentException($"Phone cannot exceed {MaxPhoneLength} characters.", nameof(phone));
    }

    private static void ValidateEmail(string? email)
    {
        if (email != null && email.Length > MaxEmailLength)
            throw new ArgumentException($"Email cannot exceed {MaxEmailLength} characters.", nameof(email));
    }

    private static void ValidateStreet(string? street)
    {
        if (street != null && street.Length > MaxStreetLength)
            throw new ArgumentException($"Street cannot exceed {MaxStreetLength} characters.", nameof(street));
    }

    private static void ValidateCity(string? city)
    {
        if (city != null && city.Length > MaxCityLength)
            throw new ArgumentException($"City cannot exceed {MaxCityLength} characters.", nameof(city));
    }

    private static void ValidateState(string? state)
    {
        if (state != null && state.Length > MaxStateLength)
            throw new ArgumentException($"State cannot exceed {MaxStateLength} characters.", nameof(state));
    }

    private static void ValidateZipCode(string? zipCode)
    {
        if (zipCode != null && zipCode.Length > MaxZipCodeLength)
            throw new ArgumentException($"Zip code cannot exceed {MaxZipCodeLength} characters.", nameof(zipCode));
    }
}
