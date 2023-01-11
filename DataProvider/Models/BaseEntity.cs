using Azure;
using Azure.Data.Tables;

namespace DataServices.Models;

public class BaseEntity : ITableEntity
{
    public string? PartitionKey { get; set; }
    public string? RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
