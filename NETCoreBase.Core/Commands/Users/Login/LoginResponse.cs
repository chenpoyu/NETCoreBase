using System;

namespace NETCoreBase.Core.Commands.Users
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool RequireChangePassword { get; set; }
    }
}
