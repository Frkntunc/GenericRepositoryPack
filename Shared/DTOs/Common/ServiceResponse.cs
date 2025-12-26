using System.Collections.Generic;

namespace Shared.DTOs.Common
{
    public class ServiceResponse
    {
        protected ServiceResponse()
        {
            Errors = new List<string>();
        }

        public bool IsSuccess { get; set; }
        public string ResponseCode { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        public static ServiceResponse Success(string code, string message = null)
        {
            return new ServiceResponse
            {
                IsSuccess = true,
                ResponseCode = code,
                Message = message
            };
        }

        public static ServiceResponse Fail(string code, string message = null, List<string> errors = null)
        {
            return new ServiceResponse
            {
                IsSuccess = false,
                ResponseCode = code,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }

    public class ServiceResponse<T> : ServiceResponse
    {
        public T? Data { get; set; }

        public static ServiceResponse<T> Success(T data, string code, string message = null)
        {
            return new ServiceResponse<T>
            {
                IsSuccess = true,
                Data = data,
                ResponseCode = code,
                Message = message
            };
        }

        public static new ServiceResponse<T> Fail(string code, string message = null, List<string> errors = null)
        {
            return new ServiceResponse<T>
            {
                IsSuccess = false,
                Data = default,
                ResponseCode = code,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
