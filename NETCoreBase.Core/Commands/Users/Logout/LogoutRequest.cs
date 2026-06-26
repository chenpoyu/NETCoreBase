using MediatR;

namespace NETCoreBase.Core.Commands.Users
{
    public class LogoutRequest : IRequest
    {
        /// <summary>
        /// Raw JWT access token to blacklist.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// UserId extracted from claims.
        /// </summary>
        public string UserId { get; set; }
    }
}
