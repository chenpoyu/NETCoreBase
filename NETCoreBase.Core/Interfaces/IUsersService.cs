using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NETCoreBase.Core.Commands.Users;
using NETCoreBase.Common.Interfaces;
using MediatR;
using NETCoreBase.Common;

namespace NETCoreBase.Core.Interfaces
{
    public interface IUsersService : IDisposable
    {
        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<LoginResponse> LoginAsync(LoginRequest req);

        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<RegisterResponse> RegisterAsync(RegisterRequest req);

        /// <summary>
        /// 查詢使用者
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<PageResult<UserListResponse>> GetUserListAsync(UserListRequest req);

        /// <summary>
        /// 用ID查詢使用者
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<UserByIdResponse> GetUserByIdAsync(UserByIdRequest req);

        /// <summary>
        /// 建立使用者
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task CreateUserAsync(CreateUserRequest req);

        /// <summary>
        /// 修改使用者
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task UpdateUserAsync(UpdateUserRequest req);

        /// <summary>
        /// 刪除使用者
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task DeleteUserAsync(DeleteUserRequest req);
    }
}


