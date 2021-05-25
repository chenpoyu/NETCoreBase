using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NETCoreBase.Core.Commands.Users
{
    public class LoginRequest : IRequest<LoginResponse>
    {
        /// <summary>
        /// 使用者帳號
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 使用者密碼
        /// </summary>
        public string UserPass { get; set; }
    }
}
