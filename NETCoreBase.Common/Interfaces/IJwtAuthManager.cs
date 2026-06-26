using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using NETCoreBase.Common.Model;

namespace NETCoreBase.Common.Interfaces
{
    public interface IJwtAuthManager
    {
        TokenResult GenerateTokens(string username, Claim[] claims, DateTime now);
        TokenResult RefreshToken(string refreshToken, DateTime now);
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
        void Blacklist(string token, DateTime expiry);
        bool IsBlacklisted(string token);
        void RevokeUserTokens(string userId);
    }
}
