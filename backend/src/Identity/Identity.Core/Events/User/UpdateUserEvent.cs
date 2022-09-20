using MediatR;

namespace Identity.Core.Events.User;

public class UpdateUserEvent: EventBase,INotification
{
    
    public string Name { get; set; }
    public string Email { get; set; }
    public long ModifiedBy { get; set; }
    public DateTimeOffset ModifiedTime { get; set; }
    public UpdateUserEvent(long id ,string name, string email, long modifiedBy, DateTimeOffset modifiedTime,long revision) : base(id, modifiedBy, revision)
    {
        Name = name;
        Email = email;
        ModifiedBy = modifiedBy;
        ModifiedTime = modifiedTime;
    }
}