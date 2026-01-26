using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AuditLog
    {
        private AuditLog(string tableName, string userId, DateTime dateTime, string type)
        {
            TableName = tableName;
            UserId = userId;
            DateTime = dateTime;
            Type = type;
        }

        public long Id { get; private set; }
        public string UserId { get; private set; }
        public string Type { get; private set; }
        public string TableName { get; private set; }
        public DateTime DateTime { get; private set; }
        public string OldValues { get; private set; } = null;
        public string NewValues { get; private set; }
        public string AffectedColumns { get; private set; } = null;
        public string PrimaryKey { get; private set; } = null;

        public static AuditLog Create(string tableName, string userId, DateTime dateTime, string type)
        {
            return new AuditLog(tableName, userId, dateTime, type);
        }

        public void SetOldValues(string oldValues)
        {
            OldValues = oldValues;
        }

        public void SetNewValues(string newValues)
        {
            NewValues = newValues;
        }

        public void SetAffectedColumns(string columns) 
        {
            AffectedColumns = columns;
        }
    }
}
