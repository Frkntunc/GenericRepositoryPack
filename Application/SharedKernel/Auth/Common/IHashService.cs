using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationService.SharedKernel.Auth.Common
{
    public interface IHashService
    {
        string Hash(string input);
        public string HashWithSalt(string input);
    }
}
