using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Common;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Roles
{
    public class RoleByIdQueryHandler : IRequestHandler<RoleByIdRequest, RoleByIdResponse>
    {
        private readonly IRolesService _rolesService;
        public RoleByIdQueryHandler(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        public async Task<RoleByIdResponse> Handle(RoleByIdRequest request, CancellationToken cancellationToken)
        {
            return await _rolesService.GetRoleByIdAsync(request);
        }
    }
}