using Domain.Events;

namespace ApplicationService.Services.Common
{
    public interface IResilientDomainEventPublisher
    {
        Task PublishAllAsync(IReadOnlyList<IDomainEvent> events, CancellationToken cancellationToken = default);
    }
}
