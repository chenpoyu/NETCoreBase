using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NETCoreBase.Core.Commands.Users
{
    public class RegisterRequest : IRequest<RegisterResponse>
    {
        /// <summary>
        /// 使用者帳號
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 使用者密碼
        /// </summary>
        public string UserPass { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string NormalizedUserName { get; set; }

        /// <summary>
        /// 電子郵件
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手機號碼
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
