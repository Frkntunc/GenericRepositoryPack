using ApplicationService.Repositories;
using MediatR;
using Shared.DTOs.Common;
using Shared.DTOs.User;
using Shared.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Features.Queries.QueryRequests.User
{
    public class GetUserByIdQuery : IRequest<ServiceResponse<UserDto>>, ICacheableRequest
    {
        public long Id { get; set; }

        public string CacheKey => CacheKeys.GetUserById + "_" + Id;

        public GetUserByIdQuery(long id) 
        {
            Id = id;
        }
    }
}
