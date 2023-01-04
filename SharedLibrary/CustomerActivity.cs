using Azure.Data.Tables;

namespace SharedLibrary;

public class CustomerActivity : BaseEntity, ITableEntity
{
    public string Description { get; set; } = string.Empty;
}
