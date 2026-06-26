using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NETCoreBase.Common.Interfaces;
using NETCoreBase.Common.Model;

namespace NETCoreBase.Common.Services
{
    public class JwtAuthManager : IJwtAuthManager
    {
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly byte[] _secret;

        private readonly ConcurrentDictionary<string, DateTime> _blacklist = new();
        private readonly ConcurrentDictionary<string, RefreshTokenEntry> _refreshTokens = new();

        private record RefreshTokenEntry(string Username, Claim[] Claims, DateTime Expiry);

        public JwtAuthManager(JwtTokenConfig jwtTokenConfig)
        {
            _jwtTokenConfig = jwtTokenConfig;
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        }

        public TokenResult GenerateTokens(string username, Claim[] claims, DateTime now)
        {
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(
                claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);

            var jwtToken = new JwtSecurityToken(
                _jwtTokenConfig.Issuer,
                shouldAddAudienceClaim ? _jwtTokenConfig.Audience : string.Empty,
                claims,
                expires: now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshExpiry = now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration);
            _refreshTokens[refreshToken] = new RefreshTokenEntry(username, claims, refreshExpiry);

            return new TokenResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
            };
        }

        public TokenResult RefreshToken(string refreshToken, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new SecurityTokenException("Invalid refresh token");

            if (!_refreshTokens.TryRemove(refreshToken, out var entry))
                throw new SecurityTokenException("Refresh token not found or already used");

            if (entry.Expiry < now)
                throw new SecurityTokenException("Refresh token has expired");

            return GenerateTokens(entry.Username, entry.Claims, now);
        }

        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new SecurityTokenException("Invalid token");

            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = _jwtTokenConfig.Issuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(_secret),
                        ValidAudience = _jwtTokenConfig.Audience,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    },
                    out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }

        public void Blacklist(string token, DateTime expiry)
        {
            if (string.IsNullOrWhiteSpace(token)) return;

            var now = DateTime.UtcNow;
            foreach (var key in _blacklist.Keys.ToList())
            {
                if (_blacklist.TryGetValue(key, out var exp) && exp < now)
                    _blacklist.TryRemove(key, out _);
            }
            _blacklist[token] = expiry.ToUniversalTime();
        }

        public bool IsBlacklisted(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;
            return _blacklist.TryGetValue(token, out var expiry) && expiry > DateTime.UtcNow;
        }

        public void RevokeUserTokens(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;
            foreach (var kvp in _refreshTokens.ToList())
            {
                if (kvp.Value.Username == userId)
                    _refreshTokens.TryRemove(kvp.Key, out _);
            }
        }
    }
}
