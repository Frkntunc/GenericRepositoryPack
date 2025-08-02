using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Abstract
{
    public interface IRefreshTokenDomainService
    {
        void ChangeRevokeStatus(RefreshToken refreshToken, bool status);
    }
}
