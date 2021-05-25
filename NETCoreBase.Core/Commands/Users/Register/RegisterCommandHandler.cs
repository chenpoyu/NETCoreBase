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
    public class RegisterCommandHandler : IRequestHandler<RegisterRequest, RegisterResponse>
    {
        private readonly IUsersService _userService;
        public RegisterCommandHandler(IUsersService userService)
        {
            _userService = userService;
        }

        public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            return await _userService.RegisterAsync(request);
        }
    }
}
