using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NETCoreBase.Core.Commands.Users
{
    public class CreateUserRequest : IRequest<Unit>
    {

        /// <summary>
        /// 使用者Id
        /// </summary>
        public Guid Id { get; set; }

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

        /// <summary>
        /// 角色
        /// </summary>
        public List<Guid> Roles { get; set; }
    }
}
