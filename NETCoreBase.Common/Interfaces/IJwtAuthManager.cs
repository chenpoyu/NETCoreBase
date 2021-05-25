using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NETCoreBase.Common.Interfaces
{
    public interface IJwtAuthManager
    {
        string GenerateTokens(string username, Claim[] claims, DateTime now);
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
    }
}
