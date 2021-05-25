using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETCoreBase.Common;
using MediatR;

namespace NETCoreBase.Core.Commands.Roles
{
    public class RoleByIdRequest : IRequest<RoleByIdResponse>
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public Guid Id { get; set; }
    }
}
