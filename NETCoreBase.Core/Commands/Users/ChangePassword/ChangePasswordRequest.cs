using MediatR;

namespace NETCoreBase.Core.Commands.Users
{
    public class ChangePasswordRequest : IRequest
    {
        /// <summary>
        /// 舊密碼
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// 新密碼
        /// </summary>
        public string NewPassword { get; set; }
    }
}
