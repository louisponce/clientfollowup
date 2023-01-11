using Azure.Data.Tables;
using System.Runtime.Serialization;

namespace DataServices.Models;

public class ActivityEntry : BaseEntity, ITableEntity
{
    public string? Id { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public string ActivityId { get; set; } = string.Empty;

    [IgnoreDataMember]
    public string ActivityDescription { get; set; } = string.Empty;
    public bool ActivityAcknowledged { get; set; }
    public decimal VendorPrice { get; set; }
    public decimal ContractorShareAmount { get; set; }
}
