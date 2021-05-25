using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NETCoreBase.Core.Commands.Roles
{
    public class UpdateRoleRequest : IRequest<Unit>
    {

        /// <summary>
        /// 角色Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 角色名稱（英文）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色名稱（中文）
        /// </summary>
        public string NormalizedName { get; set; }

        /// <summary>
        /// 使用者狀態
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 使用者
        /// </summary>
        public List<Guid> Users { get; set; }
    }
}
