using System;

namespace NETCoreBase.Core.Commands.Users
{
    public class RegisterResponse
    {
        /// <summary>
        /// 存取權杖
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 更新權杖
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// 存取權杖到期時間
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }
}
