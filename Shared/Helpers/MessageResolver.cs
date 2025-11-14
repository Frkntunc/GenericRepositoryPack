using Shared.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    public static class MessageResolver
    {
        private static readonly ResourceManager ResponseManager = ResponseMessagesResources.ResourceManager;

        public static string GetResponseMessage(string code)
        {
            var msg = ResponseManager.GetString(code.ToString());
            return string.IsNullOrWhiteSpace(msg) ? code.ToString() : msg;
        }
    }
}
