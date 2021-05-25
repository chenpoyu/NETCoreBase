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
    public class RoleListQueryHandler : IRequestHandler<RoleListRequest, PageResult<RoleListResponse>>
    {
        private readonly IRolesService _rolesService;
        public RoleListQueryHandler(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        public async Task<PageResult<RoleListResponse>> Handle(RoleListRequest request, CancellationToken cancellationToken)
        {
            return await _rolesService.GetRoleListAsync(request);
        }
    }
}