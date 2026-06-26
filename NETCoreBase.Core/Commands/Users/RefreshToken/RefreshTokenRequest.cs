using MediatR;

namespace NETCoreBase.Core.Commands.Users
{
    public class RefreshTokenRequest : IRequest<LoginResponse>
    {
        /// <summary>
        /// 更新權杖
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
