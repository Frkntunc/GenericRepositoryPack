using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ResponseCode { get; set; }
        public object? Data { get; set; }
    }

}
