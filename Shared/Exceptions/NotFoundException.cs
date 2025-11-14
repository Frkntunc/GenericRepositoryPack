using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public class NotFoundException : BaseAppException
    {
        public NotFoundException(string code) : base(code)
        {
        }
    }
}
