using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Options
{
    public class RateLimitingOptions
    {
        public int IpWindowSeconds { get; set; }
        public int IpPermitLimit { get; set; }
        public int UserPermitLimit { get; set; }
        public int UserWindowSeconds { get; set; }
        public int MaxConcurrentRequestsPerUser { get; set; }
        public int MaxConcurrentRequestsPerIp { get; set; }
        public int MaxWaitSeconds { get; set; }
    }
}
