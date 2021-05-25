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
    public class CreateUserCommandHandler : IRequestHandler<CreateUserRequest, Unit>
    {
        private readonly IUsersService _userService;

        public CreateUserCommandHandler(IUsersService userService)
        {
            _userService = userService;
        }

        public async Task<Unit> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            await _userService.CreateUserAsync(request);
            return Unit.Value;
        }
    }
}
