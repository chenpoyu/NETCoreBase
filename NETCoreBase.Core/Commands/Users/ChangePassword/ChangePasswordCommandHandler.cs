using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Users
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordRequest>
    {
        private readonly IUsersService _usersService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChangePasswordCommandHandler(IUsersService usersService, IHttpContextAccessor httpContextAccessor)
        {
            _usersService = usersService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("無法識別使用者身份");

            await _usersService.ChangePasswordAsync(userId, request);
        }
    }
}
