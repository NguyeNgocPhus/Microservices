using MediatR;

namespace Identity.Core.Events.User;

public class DeletedUserEvent : EventBase,INotification
{
    public bool Deleted { get; set; }
    
    public long ModifiedBy { get; set; }
    public DateTimeOffset ModifiedTime { get; set; }

    public DeletedUserEvent(long id , bool deleted, long modifiedBy, DateTimeOffset modifiedTime, long revision) : base(id,modifiedBy,revision)
    {
        Deleted = deleted;
        ModifiedBy = modifiedBy;
        ModifiedTime = modifiedTime;
    }
}