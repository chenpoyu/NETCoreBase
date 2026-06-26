using MediatR;

namespace NETCoreBase.Core.Commands.Users
{
    public class ResetPasswordRequest : IRequest
    {
        /// <summary>
        /// 密碼重設 Token
        /// </summary>
        public string ResetToken { get; set; }

        /// <summary>
        /// 新密碼
        /// </summary>
        public string NewPassword { get; set; }
    }
}
