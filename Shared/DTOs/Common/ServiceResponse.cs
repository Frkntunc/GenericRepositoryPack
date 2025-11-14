using Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Common
{
    public class ServiceResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string ResponseCode { get; set; }

        public static ServiceResponse<T> CreateResponse<T>(T data, string code)
        {
            return new ServiceResponse<T>
            {
                IsSuccess = code == ResponseCodes.Success,
                Data = data,
                ResponseCode = code
            };
        }
    }
}
