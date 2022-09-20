namespace Identity.Application.Services.EventStore;

public interface IEventStoreService<TAggregate> 
{
    Task StartStreamAsync(
        string streamName,
        TAggregate aggregate,
        CancellationToken cancellationToken = default (CancellationToken));
    Task AppendStreamAsync(
        string streamName,
        TAggregate aggregate,
        CancellationToken cancellationToken = default (CancellationToken));
     Task<TAggregate> AggregateStreamAsync(
        Common.Constants.EventStore.Direction direction,
        string streamName,
        long revision,
        int maxCount = 2147483647,
        CancellationToken cancellationToken = default (CancellationToken));
}