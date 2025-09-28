using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Options
{
    public class CacheOptions
    {
        public string RedisConfiguration { get; set; } = string.Empty;
        public string RedisInstanceName { get; set; } = string.Empty;
    }
}
