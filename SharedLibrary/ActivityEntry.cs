using System;
using System.Runtime.Serialization;
using Azure.Data.Tables;

namespace SharedLibrary;

public class ActivityEntry : BaseEntity, ITableEntity
{
    public string CustomerId { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public string ActivityId { get; set; } = string.Empty;

    [IgnoreDataMember]
    public string ActivityDescription { get; set; } = string.Empty;
    public bool ActivityAcknowledged { get; set; }
    public decimal VendorPrice { get; set; }
    public decimal ContractorShareAmount { get; set; }
}
