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
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleRequest, Unit>
    {
        private readonly IRolesService _roleService;
        public CreateRoleCommandHandler(IRolesService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Unit> Handle(CreateRoleRequest request, CancellationToken cancellationToken)
        {
            await _roleService.CreateRoleAsync(request);
            return Unit.Value;
        }
    }
}
