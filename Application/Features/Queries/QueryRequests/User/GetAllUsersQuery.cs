using ApplicationService.Repositories;
using MediatR;
using Shared.DTOs.Common;
using Shared.DTOs.User;
using Shared.Static;

namespace ApplicationService.Features.Queries.QueryRequests.User
{
    public class GetAllUsersQuery : IRequest<ServiceResponse<List<UserDto>>>, ICacheableRequest
    {
        public string CacheKey => CacheKeys.GetAllUsers;
        public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromMinutes(30);
    }
}
