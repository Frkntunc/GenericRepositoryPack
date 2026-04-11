using ApplicationService.Services.Common;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace ApplicationService.Services
{
    public class ResilientDomainEventPublisher : IResilientDomainEventPublisher, IScopedService
    {
        private readonly IMediator _mediator;
        private readonly IDeadLetterService _deadLetterService;
        private readonly ILogger<ResilientDomainEventPublisher> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public ResilientDomainEventPublisher(
            IMediator mediator,
            IDeadLetterService deadLetterService,
            ILogger<ResilientDomainEventPublisher> logger)
        {
            _mediator = mediator;
            _deadLetterService = deadLetterService;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<Exception>(ex => ex is not OperationCanceledException and not TaskCanceledException)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timespan, attempt, context) =>
                    {
                        _logger.LogWarning(exception,
                            "Retry attempt {Attempt} for domain event {EventType} after {Delay}s",
                            attempt, context["EventType"], timespan.TotalSeconds);
                    });
        }

        public async Task PublishAllAsync(IReadOnlyList<IDomainEvent> events, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in events)
            {
                var eventType = domainEvent.GetType().Name;

                try
                {
                    await _retryPolicy.ExecuteAsync(
                        async (ctx, ct) => await _mediator.Publish(domainEvent, ct),
                        new Context { { "EventType", eventType } },
                        cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Event publishing cancelled for {EventType}", eventType);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "All retries exhausted for domain event {EventType}. Sending to dead letter queue.",
                        eventType);

                    try
                    {
                        await _deadLetterService.SendAsync(domainEvent, ex, cancellationToken);
                    }
                    catch (Exception dlEx)
                    {
                        _logger.LogCritical(dlEx,
                            "Failed to send domain event {EventType} to dead letter service. Event data may be lost.",
                            eventType);
                    }
                }
            }
        }
    }
}
