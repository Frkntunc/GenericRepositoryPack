using Domain.Events;

namespace ApplicationService.Services.Common
{
    public interface IDomainEventCollector
    {
        IReadOnlyList<IDomainEvent> GetEvents();
        void Add(IDomainEvent domainEvent);
        void Clear();
    }
}
