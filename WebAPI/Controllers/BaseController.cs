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
    protected IActionResult CheckResponse<T>(ServiceResponse<T> response)
    {
        var httpStatus = ResponseCodeMapper.GetHttpStatus(response.ResponseCode);

        var apiResponse = new ApiResponse
        {
            Success = response.IsSuccess,
            Status = httpStatus,
            Message = MessageResolver.GetResponseMessage(response.ResponseCode),
            ResponseCode = response.ResponseCode.ToString(),
            Data = response.Data
        };

        return StatusCode(apiResponse.Status, apiResponse);
    }
}


