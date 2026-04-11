using ApplicationService.Services.Common;
using Domain.Events;

namespace ApplicationService.Services
{
    public class DomainEventCollector : IDomainEventCollector, IScopedService
    {
        private readonly List<IDomainEvent> _events = new();

        public IReadOnlyList<IDomainEvent> GetEvents() => _events.AsReadOnly();

        public void Add(IDomainEvent domainEvent) => _events.Add(domainEvent);

        public void Clear() => _events.Clear();
    }
}
