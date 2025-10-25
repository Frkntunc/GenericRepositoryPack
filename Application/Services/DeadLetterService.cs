using ApplicationService.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shared.Events;
using Shared.Options;
using Shared.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public class DeadLetterService : IDeadLetterService
    {
        private readonly ILogger<DeadLetterService> _logger;

        public DeadLetterService(ILogger<DeadLetterService> logger)
        {
            _logger = logger;
        }

        public async Task SendAsync<TRequest>(TRequest request, Exception exception)
        {
            //TODO : Burada bir kuyruğa yollama ve background service ile işleme yapılabilir. Şimdilik loglama yapılıyor.
            _logger.LogInformation($"The request could not be processed. Request : {typeof(TRequest).Name}, Ex : {exception}");

        }
    }
}
