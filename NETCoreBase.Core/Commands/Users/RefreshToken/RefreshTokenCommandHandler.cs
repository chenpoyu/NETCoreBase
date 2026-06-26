using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using NETCoreBase.Common.Exceptions;
using NETCoreBase.Common.Interfaces;

namespace NETCoreBase.Core.Commands.Users
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenRequest, LoginResponse>
    {
        private readonly IJwtAuthManager _jwtAuthManager;

        public RefreshTokenCommandHandler(IJwtAuthManager jwtAuthManager)
        {
            _jwtAuthManager = jwtAuthManager;
        }

        public Task<LoginResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var tokenResult = _jwtAuthManager.RefreshToken(request.RefreshToken, DateTime.Now);
                return Task.FromResult(new LoginResponse
                {
                    Token = tokenResult.AccessToken,
                    RefreshToken = tokenResult.RefreshToken,
                    ExpiresAt = tokenResult.ExpiresAt,
                });
            }
            catch (SecurityTokenException ex)
            {
                throw new MessageException(401, ex.Message);
            }
        }
    }
}
