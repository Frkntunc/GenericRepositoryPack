using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums
{
    public enum StatusType : short
    {
        Available = 1,

        NotAvailable = 0,

        Deleted = -1
    }
}
