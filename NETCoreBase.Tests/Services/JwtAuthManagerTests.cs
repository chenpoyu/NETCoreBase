using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using NETCoreBase.Common.Model;
using NETCoreBase.Common.Services;
using Xunit;

namespace NETCoreBase.Tests.Services
{
    public class JwtAuthManagerTests
    {
        private readonly JwtTokenConfig _config = new JwtTokenConfig
        {
            Secret = "super_secret_key_that_is_long_enough_for_hmacsha256_algorithm_min32chars",
            Issuer = "https://test.example.com/",
            Audience = "https://test.example.com/",
            AccessTokenExpiration = 60,
            RefreshTokenExpiration = 1440,
        };

        private JwtAuthManager CreateManager() => new JwtAuthManager(_config);

        [Fact]
        public void GenerateTokens_ReturnsAllFields()
        {
            var manager = CreateManager();
            var claims = new[] { new Claim("userId", Guid.NewGuid().ToString()) };

            var result = manager.GenerateTokens("testuser", claims, DateTime.UtcNow);

            Assert.NotNull(result.AccessToken);
            Assert.NotEmpty(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            Assert.NotEmpty(result.RefreshToken);
            Assert.True(result.ExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public void GenerateTokens_TwiceSameInput_DifferentRefreshTokens()
        {
            var manager = CreateManager();
            var claims = new[] { new Claim("userId", Guid.NewGuid().ToString()) };
            var now = DateTime.UtcNow;

            var r1 = manager.GenerateTokens("user", claims, now);
            var r2 = manager.GenerateTokens("user", claims, now);

            Assert.NotEqual(r1.RefreshToken, r2.RefreshToken);
        }

        [Fact]
        public void DecodeJwtToken_ValidToken_ReturnsPrincipalWithClaims()
        {
            var manager = CreateManager();
            var userId = Guid.NewGuid().ToString();
            var claims = new[]
            {
                new Claim("userId", userId),
                new Claim(ClaimTypes.Name, "testuser"),
            };

            var result = manager.GenerateTokens("testuser", claims, DateTime.UtcNow);
            var (principal, _) = manager.DecodeJwtToken(result.AccessToken);

            Assert.NotNull(principal);
            Assert.Equal("testuser", principal.Identity?.Name);
        }

        [Fact]
        public void RefreshToken_ValidToken_ReturnsNewTokenPair()
        {
            var manager = CreateManager();
            var claims = new[] { new Claim("userId", Guid.NewGuid().ToString()) };
            var initial = manager.GenerateTokens("testuser", claims, DateTime.UtcNow);

            var refreshed = manager.RefreshToken(initial.RefreshToken, DateTime.UtcNow);

            Assert.NotNull(refreshed.AccessToken);
            Assert.NotNull(refreshed.RefreshToken);
            Assert.NotEqual(initial.RefreshToken, refreshed.RefreshToken);
        }

        [Fact]
        public void RefreshToken_UsedToken_ThrowsSecurityTokenException()
        {
            var manager = CreateManager();
            var claims = new[] { new Claim("userId", Guid.NewGuid().ToString()) };
            var initial = manager.GenerateTokens("testuser", claims, DateTime.UtcNow);

            manager.RefreshToken(initial.RefreshToken, DateTime.UtcNow);

            Assert.Throws<SecurityTokenException>(() =>
                manager.RefreshToken(initial.RefreshToken, DateTime.UtcNow));
        }

        [Fact]
        public void RefreshToken_UnknownToken_ThrowsSecurityTokenException()
        {
            var manager = CreateManager();

            Assert.Throws<SecurityTokenException>(() =>
                manager.RefreshToken("unknown_token_value", DateTime.UtcNow));
        }

        [Fact]
        public void RefreshToken_ExpiredEntry_ThrowsSecurityTokenException()
        {
            var manager = CreateManager();
            var claims = new[] { new Claim("userId", Guid.NewGuid().ToString()) };
            // Issue the token so expiry = past_time + 1440 min = still in past if past_time is far enough back
            var pastTime = DateTime.UtcNow.AddMinutes(-(_config.RefreshTokenExpiration + 10));
            var initial = manager.GenerateTokens("testuser", claims, pastTime);

            Assert.Throws<SecurityTokenException>(() =>
                manager.RefreshToken(initial.RefreshToken, DateTime.UtcNow));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void RefreshToken_NullOrWhitespace_ThrowsSecurityTokenException(string token)
        {
            var manager = CreateManager();

            Assert.Throws<SecurityTokenException>(() =>
                manager.RefreshToken(token, DateTime.UtcNow));
        }

        [Fact]
        public void DecodeJwtToken_EmptyToken_ThrowsSecurityTokenException()
        {
            var manager = CreateManager();

            Assert.Throws<SecurityTokenException>(() =>
                manager.DecodeJwtToken(string.Empty));
        }
    }
}
