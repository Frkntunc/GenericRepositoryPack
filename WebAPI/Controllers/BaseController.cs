using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using Shared.DTOs.Common;
using Shared.Helpers;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult CheckResponse(ServiceResponse response)
    {
        var httpStatus = ResponseCodeMapper.GetHttpStatus(response.ResponseCode);
        var message = !string.IsNullOrEmpty(response.Message)
                      ? response.Message
                      : MessageResolver.GetResponseMessage(response.ResponseCode);

        var apiResponse = new ApiResponse
        {
            Success = response.IsSuccess,
            Status = httpStatus,
            Message = message,
            ResponseCode = response.ResponseCode,
            Errors = response.Errors
        };

        return StatusCode(apiResponse.Status, apiResponse);
    }

    protected IActionResult CheckResponse<T>(ServiceResponse<T> response)
    {
        var httpStatus = ResponseCodeMapper.GetHttpStatus(response.ResponseCode);
        var message = !string.IsNullOrEmpty(response.Message)
                      ? response.Message
                      : MessageResolver.GetResponseMessage(response.ResponseCode);

        var apiResponse = new ApiResponse<T>
        {
            Success = response.IsSuccess,
            Status = httpStatus,
            Message = message,
            ResponseCode = response.ResponseCode,
            Data = response.Data,
            Errors = response.Errors
        };

        return StatusCode(apiResponse.Status, apiResponse);
    }
}


