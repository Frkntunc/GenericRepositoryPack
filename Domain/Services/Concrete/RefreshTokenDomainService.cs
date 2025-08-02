using Domain.Entities;
using Domain.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Concrete
{
    public class RefreshTokenDomainService : IRefreshTokenDomainService
    {

        public void ChangeRevokeStatus(RefreshToken refreshToken, bool status)
        {
            refreshToken.IsRevoked = status;
        }
    }
}
