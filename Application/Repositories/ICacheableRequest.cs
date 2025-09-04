using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Repositories
{
    public interface ICacheableRequest
    {
        string CacheKey { get; }
        TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromMinutes(30);
    }

}
