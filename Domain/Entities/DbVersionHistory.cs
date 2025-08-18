using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DbVersionHistory
    {
        public int Id { get; set; }
        public string MigrationName { get; set; } = null!;
        public DateTime AppliedOn { get; set; } = DateTime.Now;
        public string AppliedBy { get; set; } = "System";
        public string AppVersion { get; set; } = null!;
        public string MachineName { get; set; } = null!;
        public TimeSpan? Duration { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? ErrorMessage { get; set; }
    }

}
