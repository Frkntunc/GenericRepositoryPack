using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Repositories
{
    public interface IDeadLetterService
    {
        Task SendAsync<TRequest>(TRequest request, Exception exception);
    }
}
