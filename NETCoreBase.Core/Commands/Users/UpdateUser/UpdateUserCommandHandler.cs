using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Core.Commands;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Users
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserRequest, Unit>
    {
        private readonly IUsersService _usersService;

        public UpdateUserCommandHandler(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task<Unit> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            await _usersService.UpdateUserAsync(request);
            return Unit.Value;
        }

    }
}
