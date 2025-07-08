using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Features.Queries.QueryRequests.User
{
    public class GetAllUsersQuery : IRequest<List<UserReadModel>> 
    {
        
    }
}
