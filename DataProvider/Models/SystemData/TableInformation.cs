using Azure.Data.Tables;

namespace DataServices.Models.SystemData;

public class TableInformation: BaseEntity, ITableEntity
{
    public int LineCount { get; set; }
    public string? LastRowKey { get; set; }
}
