using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Core.Commands;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Roles
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleRequest, Unit>
    {
        private readonly IRolesService _rolesService;
        public UpdateRoleCommandHandler(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        public async Task<Unit> Handle(UpdateRoleRequest request, CancellationToken cancellationToken)
        {
            await _rolesService.UpdateRoleAsync(request);
            return Unit.Value;
        }

    }
}
