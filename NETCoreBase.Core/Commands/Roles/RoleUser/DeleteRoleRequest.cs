using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NETCoreBase.Core.Commands.Roles
{
    public class DeleteRoleRequest : IRequest<Unit>
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public List<Guid> Id { get; set; }
    }
}
