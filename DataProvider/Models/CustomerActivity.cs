using Azure.Data.Tables;

namespace DataServices.Models;

public class CustomerActivity : BaseEntity, ITableEntity

{
    public string? Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
