using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Users
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordRequest>
    {
        private readonly IUsersService _usersService;

        public ResetPasswordCommandHandler(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            await _usersService.ResetPasswordAsync(request);
        }
    }
}
