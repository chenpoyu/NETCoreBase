using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NETCoreBase.Core.Commands.Roles;
using NETCoreBase.Common.Interfaces;
using MediatR;
using NETCoreBase.Common;

namespace NETCoreBase.Core.Interfaces
{
    public interface IRolesService
    {
        /// <summary>
        /// 查詢角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<PageResult<RoleListResponse>> GetRoleListAsync(RoleListRequest req);

        /// <summary>
        /// 用ID查詢角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<RoleByIdResponse> GetRoleByIdAsync(RoleByIdRequest req);

        /// <summary>
        /// 建立角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task CreateRoleAsync(CreateRoleRequest req);

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task UpdateRoleAsync(UpdateRoleRequest req);

        /// <summary>
        /// 刪除角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task DeleteRoleAsync(DeleteRoleRequest req);
    }
}


