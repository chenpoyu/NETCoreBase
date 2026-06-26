using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Common.Interfaces;

namespace NETCoreBase.Core.Commands.Users
{
    public class LogoutCommandHandler : IRequestHandler<LogoutRequest>
    {
        private readonly IJwtAuthManager _jwtAuthManager;

        public LogoutCommandHandler(IJwtAuthManager jwtAuthManager)
        {
            _jwtAuthManager = jwtAuthManager;
        }

        public Task Handle(LogoutRequest request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.Token))
            {
                // Parse expiry so the blacklist entry can be cleaned up later
                var handler = new JwtSecurityTokenHandler();
                DateTime expiry = DateTime.UtcNow.AddMinutes(5); // safe fallback
                if (handler.CanReadToken(request.Token))
                {
                    var jwt = handler.ReadJwtToken(request.Token);
                    expiry = jwt.ValidTo;
                }
                _jwtAuthManager.Blacklist(request.Token, expiry);
            }

            if (!string.IsNullOrWhiteSpace(request.UserId))
            {
                _jwtAuthManager.RevokeUserTokens(request.UserId);
            }

            return Task.CompletedTask;
        }
    }
}
