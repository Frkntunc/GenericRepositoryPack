using ApplicationService.Repositories;
using MediatR;
using Shared.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.DTO;

namespace ApplicationService.Features.Queries.QueryRequests.User
{
    public class GetUserByIdQuery : IRequest<UserDto>, ICacheableRequest
    {
        public long Id { get; set; }

        public string CacheKey => CacheKeys.GetUserById + "_" + Id;

        public GetUserByIdQuery(long id) 
        {
            Id = id;
        }
    }
}
