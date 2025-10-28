using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Repositories
{
    public interface IRateLimitService
    {
        Task<(bool Allowed, long Current, long Remaining, long ResetSeconds)> TryAcquireAsync(string key, int permitLimit, TimeSpan window);
    }
}
