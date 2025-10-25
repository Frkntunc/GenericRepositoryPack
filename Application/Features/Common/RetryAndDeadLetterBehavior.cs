using ApplicationService.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Reflection;

namespace ApplicationService.Features.Common
{
    namespace Application.Common.Behaviors
    {
        public class RetryAndDeadLetterBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            private readonly ILogger<RetryAndDeadLetterBehavior<TRequest, TResponse>> _logger;
            private readonly IDeadLetterService _deadLetterService;
            private readonly AsyncRetryPolicy _retryPolicy;

            public RetryAndDeadLetterBehavior(
                ILogger<RetryAndDeadLetterBehavior<TRequest, TResponse>> logger,
                IDeadLetterService deadLetterService)
            {
                _logger = logger;
                _deadLetterService = deadLetterService;

                // NOT: OperationCanceledException / TaskCanceledException'i retry etmiyoruz
                _retryPolicy = Policy
                    .Handle<Exception>(ex => !(ex is OperationCanceledException || ex is TaskCanceledException))
                    .WaitAndRetryAsync(
                        retryCount: 3,
                        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // 2s,4s,8s
                        onRetry: (exception, timespan, attempt, context) =>
                        {
                            _logger.LogWarning(exception,
                                "Retry attempt {Attempt} for {RequestName} after {Delay}s",
                                attempt, typeof(TRequest).Name, timespan.TotalSeconds);
                        });
            }

            public async Task<TResponse> Handle(
                TRequest request,
                RequestHandlerDelegate<TResponse> next,
                CancellationToken cancellationToken)
            {
                var hasAttribute = request.GetType().GetCustomAttribute<EnableRetryAndDlqAttribute>() != null;

                if (!hasAttribute)
                    return await next();

                try
                {
                    return await _retryPolicy.ExecuteAsync((ct) => next(), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Cancellation durumunda DLQ'ya yollama.
                    _logger.LogInformation("Operation cancelled for {RequestName}", typeof(TRequest).Name);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "All retries failed for {RequestName}. Sending to Dead Letter Queue.", typeof(TRequest).Name);

                    try
                    {
                        await _deadLetterService.SendAsync(request, ex);
                    }
                    catch (Exception dlEx)
                    {
                        _logger.LogError(dlEx, "Failed to send request to Dead Letter Service for {RequestName}", typeof(TRequest).Name);
                    }

                    throw;
                }
            }
        }

        //Retry ve DLQ mekanizmasını etkinleştirmek istediğimiz request'lere bu attribute'u ekliyoruz.
        [AttributeUsage(AttributeTargets.Class)]
        public class EnableRetryAndDlqAttribute : Attribute
        {
        }

    }

}
