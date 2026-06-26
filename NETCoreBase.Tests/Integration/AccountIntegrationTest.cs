using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace NETCoreBase.Tests.Integration
{
    /// <summary>
    /// Integration tests for the Account (login/logout/register) flow.
    /// Tests run against a real SQL Server container via Testcontainers.
    /// </summary>
    public class AccountIntegrationTest : BaseIntegrationTest
    {
        public AccountIntegrationTest(SqlServerFixture fixture) : base(fixture) { }

        // ── Test 1: Register → Login → use access token ─────────────────────────

        [Fact]
        public async Task Register_Login_Token_Flow()
        {
            var username = $"test_{Guid.NewGuid():N}";
            const string password = "P@ssword1!";

            // Register
            var registerResp = await Client.PostAsync("/api/Account/Register", Json(new
            {
                userName = username,
                userPass = password,
                normalizedUserName = "Test User",
                email = $"{username}@example.com",
                phoneNumber = "0912345678"
            }));
            Assert.Equal(HttpStatusCode.OK, registerResp.StatusCode);
            var registerBody = await registerResp.Content.ReadAsStringAsync();
            var registerJson = JsonNode.Parse(registerBody);
            var token = registerJson?["token"]?.GetValue<string>();
            Assert.False(string.IsNullOrWhiteSpace(token), "Token must be returned after registration");

            // Login with the registered account
            var loginResp = await Client.PostAsync("/api/Account/Login", Json(new
            {
                userName = username,
                userPass = password
            }));
            Assert.Equal(HttpStatusCode.OK, loginResp.StatusCode);
            var loginBody = await loginResp.Content.ReadAsStringAsync();
            var loginJson = JsonNode.Parse(loginBody);
            var accessToken = loginJson?["token"]?.GetValue<string>();
            Assert.False(string.IsNullOrWhiteSpace(accessToken), "Access token must be returned on login");

            // Use the access token on a protected endpoint
            var req = new HttpRequestMessage(HttpMethod.Get, "/api/Users");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var protectedResp = await Client.SendAsync(req);
            // Should be 200 (or at minimum not 401)
            Assert.NotEqual(HttpStatusCode.Unauthorized, protectedResp.StatusCode);
        }

        // ── Test 2: 5 failed logins → account locked (423) ──────────────────────

        [Fact]
        public async Task Login_Fail_5_Times_AccountLocked()
        {
            var username = $"locktest_{Guid.NewGuid():N}";
            const string password = "P@ssword1!";

            // Register the user first
            var regResp = await Client.PostAsync("/api/Account/Register", Json(new
            {
                userName = username,
                userPass = password,
                normalizedUserName = "Lock Test",
                email = $"{username}@example.com",
                phoneNumber = "0987654321"
            }));
            Assert.Equal(HttpStatusCode.OK, regResp.StatusCode);

            // Fail login 5 times with wrong password
            for (int i = 0; i < 5; i++)
            {
                var resp = await Client.PostAsync("/api/Account/Login", Json(new
                {
                    userName = username,
                    userPass = "WrongPassword!"
                }));
                // Each failed attempt should return 401
                Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
            }

            // 6th attempt (even with correct password) should be locked (423)
            var lockedResp = await Client.PostAsync("/api/Account/Login", Json(new
            {
                userName = username,
                userPass = password
            }));
            Assert.Equal((HttpStatusCode)423, lockedResp.StatusCode);
        }

        // ── Test 3: Login → Logout → old token rejected (401) ───────────────────

        [Fact]
        public async Task Login_Logout_OldToken_Rejected()
        {
            var username = $"logout_{Guid.NewGuid():N}";
            const string password = "P@ssword1!";

            // Register
            await Client.PostAsync("/api/Account/Register", Json(new
            {
                userName = username,
                userPass = password,
                normalizedUserName = "Logout Test",
                email = $"{username}@example.com",
                phoneNumber = "0911111111"
            }));

            // Login
            var loginResp = await Client.PostAsync("/api/Account/Login", Json(new
            {
                userName = username,
                userPass = password
            }));
            Assert.Equal(HttpStatusCode.OK, loginResp.StatusCode);
            var loginBody = await loginResp.Content.ReadAsStringAsync();
            var loginJson = JsonNode.Parse(loginBody);
            var token = loginJson?["token"]?.GetValue<string>();
            Assert.False(string.IsNullOrWhiteSpace(token));

            // Logout
            var logoutReq = new HttpRequestMessage(HttpMethod.Post, "/api/Account/Logout");
            logoutReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var logoutResp = await Client.SendAsync(logoutReq);
            Assert.Equal(HttpStatusCode.OK, logoutResp.StatusCode);

            // Old token should now be rejected with 401
            var afterLogoutReq = new HttpRequestMessage(HttpMethod.Get, "/api/Users");
            afterLogoutReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var afterLogoutResp = await Client.SendAsync(afterLogoutReq);
            Assert.Equal(HttpStatusCode.Unauthorized, afterLogoutResp.StatusCode);
        }
    }
}
