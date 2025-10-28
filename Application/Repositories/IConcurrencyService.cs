using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Repositories
{
    public interface IConcurrencyService
    {
        Task<bool> TryEnterAsync(string key, int maxConcurrent, TimeSpan ttl);
        Task ExitAsync(string key);
    }
}
