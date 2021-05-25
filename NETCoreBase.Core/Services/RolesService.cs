using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NETCoreBase.Core.Commands;
using NETCoreBase.Core.Interfaces;
using NETCoreBase.Common.Interfaces;
using NETCoreBase.Common.Services;
using NETCoreBase.Database;
using NETCoreBase.Database.Models;
using NETCoreBase.Common.Helpers;
using NETCoreBase.Core.Commands.Roles;
using NETCoreBase.Common;
using NETCoreBase.Common.Exceptions;

namespace NETCoreBase.Core.Services
{
    public class RolesService : GenericRepository<Role>, IRolesService
    {
        private readonly ILogger<RolesService> _logger;
        private readonly NETCoreBaseContext _context;
        private readonly IMapper _mapper;
        private readonly ClaimsPrincipal _clamis;

        public RolesService(ILogger<RolesService> logger, NETCoreBaseContext context, 
            IMapper mapper, ClaimsPrincipal clamis) 
            : base(context, mapper, clamis)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _clamis = clamis;
        }

        /// <summary>
        /// 查詢角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<PageResult<RoleListResponse>> GetRoleListAsync(RoleListRequest req)
        {
            return await base.QueryAsync<RoleListResponse>(req.GetExpression(), req);
        }

        /// <summary>
        /// 用ID查詢角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<RoleByIdResponse> GetRoleByIdAsync(RoleByIdRequest req)
        {
            var role = await base.FirstOrDefaultAsync<RoleByIdResponse>(u => u.Id == req.Id);
            if (role == null)
            {
                throw new MessageException(404, "無此角色");
            }
            return role;
        }

        /// <summary>
        /// 建立角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task CreateRoleAsync(CreateRoleRequest req)
        {
            var role = await base.InsertAsync(req);

            await UpdateUserRoleAsync(role.Id, req.Users);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task UpdateRoleAsync(UpdateRoleRequest req)
        {
            var role = await base.FirstOrDefaultAsync<Role>(u => u.Id == req.Id);

            if (role == null)
            {
                throw new MessageException(400, "無此角色");
            }
            
            role.Name = req.Name;
            role.NormalizedName = req.NormalizedName;
            role.Status = req.Status ?? "A";

            await base.UpdateAsync(role);

            await UpdateUserRoleAsync(role.Id, req.Users);
        }

        /// <summary>
        /// 刪除角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task DeleteRoleAsync(DeleteRoleRequest req)
        {
            var roles = await base.QueryAsync<Role>(u => req.Id.Contains(u.Id));
            foreach (var u in roles)
            {
                u.Status = "D";
            }
            await base.UpdateRangeAsync(roles);
        }

        /// <summary>
        /// 建立角色擁有的使用者
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        private async Task UpdateUserRoleAsync(Guid roleId, List<Guid> users)
        {
            if (users != null && users.Count > 0)
            {
                var list = await _context.UserRoles.Where(r => r.RoleId == roleId).ToListAsync();

                var adds = users.Where(u => !list.Any(l => l.UserId == u)).ToList();
                var removes = list.Where(l => !users.Any(u => u == l.UserId)).ToList();
                // var edits = roles.Where(r => !adds.Any(a => a == r)).ToList();

                List<UserRole> urlist = new List<UserRole>();
                foreach (var a in adds)
                {
                    urlist.Add(new UserRole()
                    {
                        UserId = a,
                        RoleId = roleId,
                    });
                }
                _context.AddRange(urlist);
                _context.RemoveRange(removes);

                await SaveChangesAsync();
            }
        }
    }
}
