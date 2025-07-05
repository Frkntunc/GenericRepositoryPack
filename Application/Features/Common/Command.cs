using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Features.Common
{
    public abstract class Command
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }

}
