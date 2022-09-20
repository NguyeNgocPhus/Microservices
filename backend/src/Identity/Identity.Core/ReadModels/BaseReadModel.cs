namespace Identity.Core.ReadModels;

public class BaseReadModel
{
    public long  Id { get; set; }
    public long CreatedBy { get; set; }
    public long? ModifiedBy { get; set; }
    public bool? Deleted{ get; set; }
    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset? ModifiedTime { get; set; }
    public DateTimeOffset? DeletedTime { get; set; }
}