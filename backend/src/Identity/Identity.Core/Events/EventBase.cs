using MediatR;

namespace Identity.Core.Events;

public class EventBase
{
    public long Id { get; set; }
    public long CreatedBy { get; set; }
    public long Revision { get; set; }

    public EventBase(long id , long createdBy, long revision)
    {
        Id = id;
        CreatedBy = createdBy;
        Revision = revision;
    }


}