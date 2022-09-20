namespace Identity.Core.Aggregates;

public class EventHistoryItem
{
    public string EventType { get; set; } = string.Empty;
    public long EventRevision { get; set; }
    public long CreatedBy { get; set; }
}