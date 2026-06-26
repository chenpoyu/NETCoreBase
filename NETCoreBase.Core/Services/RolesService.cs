using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NETCoreBase.Core.Interfaces;
using NETCoreBase.Common.Interfaces;
using NETCoreBase.Database;
using NETCoreBase.Database.Models;
using NETCoreBase.Core.Commands.Roles;
using NETCoreBase.Common;
using NETCoreBase.Common.Exceptions;

namespace NETCoreBase.Core.Services
{
    public class RolesService : IRolesService
    {
        private readonly ILogger<RolesService> _logger;
        private readonly NETCoreBaseContext _context;
        private readonly IGenericRepository<Role> _repository;

        public RolesService(ILogger<RolesService> logger, NETCoreBaseContext context,
            IGenericRepository<Role> repository)
        {
            _logger = logger;
            _context = context;
            _repository = repository;
        }

        public async Task<PageResult<RoleListResponse>> GetRoleListAsync(RoleListRequest req)
        {
            return await _repository.QueryAsync<RoleListResponse>(req.GetExpression(), req);
        }

        public async Task<RoleByIdResponse> GetRoleByIdAsync(RoleByIdRequest req)
        {
            var role = await _repository.FirstOrDefaultAsync<RoleByIdResponse>(u => u.Id == req.Id);
            if (role == null)
            {
                throw new MessageException(404, "無此角色");
            }
            return role;
        }

        public async Task CreateRoleAsync(CreateRoleRequest req)
        {
            var role = await _repository.InsertAsync(req);
            await UpdateUserRoleAsync(role.Id, req.Users);
        }

        public async Task UpdateRoleAsync(UpdateRoleRequest req)
        {
            var role = await _repository.FirstOrDefaultAsync<Role>(u => u.Id == req.Id);

            if (role == null)
            {
                throw new MessageException(400, "無此角色");
            }

            role.Name = req.Name;
            role.NormalizedName = req.NormalizedName;
            role.Status = req.Status ?? "A";

            await _repository.UpdateAsync(role);

            await UpdateUserRoleAsync(role.Id, req.Users);
        }

        public async Task DeleteRoleAsync(DeleteRoleRequest req)
        {
            var roles = await _repository.QueryAsync<Role>(u => req.Id.Contains(u.Id));
            foreach (var u in roles)
            {
                u.Status = "D";
            }
            await _repository.UpdateRangeAsync(roles);
        }

        private async Task UpdateUserRoleAsync(Guid roleId, List<Guid> users)
        {
            if (users != null && users.Count > 0)
            {
                var list = await _context.UserRoles.Where(r => r.RoleId == roleId).ToListAsync();

                var adds = users.Where(u => !list.Any(l => l.UserId == u)).ToList();
                var removes = list.Where(l => !users.Any(u => u == l.UserId)).ToList();

                var urlist = adds.Select(a => new UserRole { UserId = a, RoleId = roleId }).ToList();
                _context.AddRange(urlist);
                _context.RemoveRange(removes);

                await _repository.SaveAsync();
            }
        }
    }
}
