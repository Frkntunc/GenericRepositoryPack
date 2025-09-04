using ApplicationService.Repositories;
using MediatR;
using Users.Application.DTO;
using Shared.Static;

namespace ApplicationService.Features.Queries.QueryRequests.User
{
    public class GetAllUsersQuery : IRequest<List<UserDto>>, ICacheableRequest
    {
        public string CacheKey => CacheKeys.GetAllUsers;
        public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromMinutes(30);
    }
}
