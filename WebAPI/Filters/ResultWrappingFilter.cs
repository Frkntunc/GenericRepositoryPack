using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace WebAPI.Filters
{
    public class ResultWrappingFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.Value is ApiResponse<object> || objectResult.Value?.GetType().Name.StartsWith("ApiResponse") == true)
                    return;

                var wrapped = new ApiResponse<object>
                {
                    Success = true,
                    Data = objectResult.Value,
                    Message = "İşlem başarılı",
                    Code = 0,
                    Status = objectResult.StatusCode ?? 200
                };

                context.Result = new ObjectResult(wrapped)
                {
                    StatusCode = objectResult.StatusCode
                };
            }
        }
    }
}
