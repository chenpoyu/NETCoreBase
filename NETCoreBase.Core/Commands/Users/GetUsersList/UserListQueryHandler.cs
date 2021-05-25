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
    public class UserListQueryHandler : IRequestHandler<UserListRequest, PageResult<UserListResponse>>
    {
        private readonly IUsersService _usersService;
        public UserListQueryHandler(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task<PageResult<UserListResponse>> Handle(UserListRequest request, CancellationToken cancellationToken)
        {
            return await _usersService.GetUserListAsync(request);
        }
    }
}