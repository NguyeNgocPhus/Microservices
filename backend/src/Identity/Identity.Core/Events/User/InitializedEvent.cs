using MediatR;

namespace Identity.Core.Events.User;

public class InitializedUserEvent : EventBase,INotification
{
    public InitializedUserEvent(long id,string name, string email, string password, long createdBy, DateTimeOffset createdTime, long revision)
    :base (id,createdBy,revision)
    {
        Email = email;
        Password = password;
        Name = name;
        CreatedBy = createdBy;
        CreatedTime = createdTime;
    }

   
    public string Email { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public long CreatedBy { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
    
    
   
    
    
}