namespace SharedLibrary;

public class Customer
{
    public string? Id { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public string? City { get; set; } = string.Empty;
    public string? PostCode { get; set; } = string.Empty;
    public string? CountryCode { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public bool Active { get; set; }
}
