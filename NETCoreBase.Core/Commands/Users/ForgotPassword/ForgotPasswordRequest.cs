using MediatR;

namespace NETCoreBase.Core.Commands.Users
{
    public class ForgotPasswordRequest : IRequest<ForgotPasswordResponse>
    {
        /// <summary>
        /// 帳號（使用者名稱）
        /// </summary>
        public string Username { get; set; }
    }
}
