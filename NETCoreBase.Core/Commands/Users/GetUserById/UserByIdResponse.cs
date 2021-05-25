using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETCoreBase.Core.Commands.Users
{
    public class UserByIdResponse
    {
        /// <summary>
        /// 使用者Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 使用者帳號
        /// </summary>
        [DisplayName("帳號")]
        public string UserName { get; set; }

        /// <summary>
        /// 使用者姓名
        /// </summary>
        [DisplayName("姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 使用者電子郵件
        /// </summary>
        [DisplayName("電子郵件")]
        public string Email { get; set; }

        /// <summary>
        /// 使用者手機號碼
        /// </summary>
        [DisplayName("手機號碼")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 使用者狀態
        /// </summary>
        [DisplayName("狀態")]
        public string Status { get; set; }
    }
}
