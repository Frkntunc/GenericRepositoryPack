using ApplicationService.Repositories.Common;
using ApplicationService.Services.Common;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Filters
{
    public class UnitOfWorkSaveChangesFilter : IAsyncActionFilter
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainEventCollector _eventCollector;
        private readonly IResilientDomainEventPublisher _eventPublisher;

        public UnitOfWorkSaveChangesFilter(
            IUnitOfWork unitOfWork,
            IDomainEventCollector eventCollector,
            IResilientDomainEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _eventCollector = eventCollector;
            _eventPublisher = eventPublisher;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (resultContext.Exception == null || resultContext.ExceptionHandled)
            {
                await _unitOfWork.SaveChangesAsync(context.HttpContext.RequestAborted);

                var events = _eventCollector.GetEvents();

                if (events.Count > 0)
                {
                    await _eventPublisher.PublishAllAsync(events);
                    _eventCollector.Clear();
                }
            }
        }
    }
}
