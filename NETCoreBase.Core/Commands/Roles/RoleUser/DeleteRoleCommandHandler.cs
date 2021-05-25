using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Roles
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleRequest, Unit>
    {
        private readonly IRolesService _roleService;
        public DeleteRoleCommandHandler(IRolesService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Unit> Handle(DeleteRoleRequest request, CancellationToken cancellationToken)
        {
            await _roleService.DeleteRoleAsync(request);
            return Unit.Value;
        }
    }
}
