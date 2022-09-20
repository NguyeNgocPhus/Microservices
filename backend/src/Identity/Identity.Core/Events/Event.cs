using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Core.Events;

[Table("EventStores")]
public class Event 
{
    public Event(){}
    
    public long Id { get; set; }
    public string StreamName { get; set; }
    public long Revision { get; set; }
    public string Type { get; set; }
    [Column(TypeName = "jsonb")]
    public string Data { get; set; }
    public uint ConcurrencyStamp { get; set; }

    public DateTimeOffset LastModified { get; set; }
    public long CreatedBy { get; set; }

    public Event(
        string streamName,
        long revision,
        string type,
        string data,
        long createdBy
     )
    {
        StreamName = streamName;
        Revision = revision;
        Type = type;
        Data = data;
        LastModified = DateTimeOffset.UtcNow;
        CreatedBy = createdBy;
    }

    
}