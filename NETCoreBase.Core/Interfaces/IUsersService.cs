using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NETCoreBase.Core.Commands.Users;
using NETCoreBase.Common.Interfaces;
using MediatR;
using NETCoreBase.Common;

namespace NETCoreBase.Core.Interfaces
{
    public interface IUsersService
    {
        /// <summary>登入</summary>
        Task<LoginResponse> LoginAsync(LoginRequest req);

        /// <summary>註冊</summary>
        Task<RegisterResponse> RegisterAsync(RegisterRequest req);

        /// <summary>變更密碼</summary>
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequest req);

        /// <summary>忘記密碼（產生重設 Token）</summary>
        Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest req);

        /// <summary>重設密碼</summary>
        Task ResetPasswordAsync(ResetPasswordRequest req);

        /// <summary>查詢使用者</summary>
        Task<PageResult<UserListResponse>> GetUserListAsync(UserListRequest req);

        /// <summary>用ID查詢使用者</summary>
        Task<UserByIdResponse> GetUserByIdAsync(UserByIdRequest req);

        /// <summary>建立使用者</summary>
        Task CreateUserAsync(CreateUserRequest req);

        /// <summary>修改使用者</summary>
        Task UpdateUserAsync(UpdateUserRequest req);

        /// <summary>刪除使用者</summary>
        Task DeleteUserAsync(DeleteUserRequest req);
    }
}
