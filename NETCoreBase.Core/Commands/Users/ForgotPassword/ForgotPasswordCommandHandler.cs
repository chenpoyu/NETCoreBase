using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Users
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordRequest, ForgotPasswordResponse>
    {
        private readonly IUsersService _usersService;

        public ForgotPasswordCommandHandler(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task<ForgotPasswordResponse> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            return await _usersService.ForgotPasswordAsync(request);
        }
    }
}
