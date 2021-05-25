using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Common;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Users
{
    public class UserByIdQueryHandler : IRequestHandler<UserByIdRequest, UserByIdResponse>
    {
        private readonly IUsersService _usersService;
        public UserByIdQueryHandler(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task<UserByIdResponse> Handle(UserByIdRequest request, CancellationToken cancellationToken)
        {
            return await _usersService.GetUserByIdAsync(request);
        }
    }
}