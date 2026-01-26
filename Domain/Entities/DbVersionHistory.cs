using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DbVersionHistory
    {
        private DbVersionHistory(string migrationName, DateTime appliedOn, string appliedBy, string appVersion, string machineName, TimeSpan? duration, string status, string? errorMessage)
        {
            MigrationName = migrationName;
            AppliedOn = appliedOn;
            AppliedBy = appliedBy;
            AppVersion = appVersion;
            MachineName = machineName;
            Duration = duration;
            Status = status;
            ErrorMessage = errorMessage;
        }

        public int Id { get; private set; }
        public string MigrationName { get; private set; } = null!;
        public DateTime AppliedOn { get; private set; } = DateTime.Now;
        public string AppliedBy { get; private set; } = "System";
        public string AppVersion { get; private set; } = null!;
        public string MachineName { get; private set; } = null!;
        public TimeSpan? Duration { get; private set; } = null!;
        public string Status { get; private set; } = null!;
        public string? ErrorMessage { get; private set; }

        public static DbVersionHistory Create(string migrationName, DateTime appliedOn, string appliedBy, string appVersion, string machineName, TimeSpan? duration, string status, string? errorMessage)
        {
            return new DbVersionHistory(migrationName, appliedOn, appliedBy, appVersion, machineName, duration, status, errorMessage);
        }
    }

}
