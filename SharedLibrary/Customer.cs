using Azure.Data.Tables;

namespace SharedLibrary;

public class Customer : BaseEntity, ITableEntity
{
    public string? Name { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public string? City { get; set; } = string.Empty;
    public string? PostCode { get; set; } = string.Empty;
    public string? CountryCode { get; set; } = string.Empty;
}
