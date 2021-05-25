using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Users
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserRequest, Unit>
    {
        private readonly IUsersService _userService;
        public DeleteUserCommandHandler(IUsersService userService)
        {
            _userService = userService;
        }

        public async Task<Unit> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            await _userService.DeleteUserAsync(request);
            return Unit.Value;
        }
    }
}
