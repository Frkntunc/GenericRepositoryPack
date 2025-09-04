using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public record CacheInvalidatedEvent
    {
        public string CacheKey { get; set; }
        public DateTime PublishedOn { get; set; } = DateTime.Now;

        public CacheInvalidatedEvent(string cacheKey)
        {
            CacheKey = cacheKey;
        }
    }

}
