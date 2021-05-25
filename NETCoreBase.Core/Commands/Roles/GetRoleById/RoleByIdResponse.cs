using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETCoreBase.Core.Commands.Roles
{
    public class RoleByIdResponse
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 角色名稱（英文）
        /// </summary>
        [DisplayName("角色名稱")]
        public string Name { get; set; }

        /// <summary>
        /// 角色名稱（中文）
        /// </summary>
        [DisplayName("角色名稱")]
        public string NormalizedName { get; set; }

        /// <summary>
        /// 角色狀態
        /// </summary>
        [DisplayName("狀態")]
        public string Status { get; set; }
    }
}
