using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Options
{
    public class QueueOptions
    {
        public string RabbitMqHost { get; set; } = string.Empty;
        public string RabbitMqUsername { get; set; } = string.Empty;
        public string RabbitMqPassword { get; set; } = string.Empty;
    }
}
