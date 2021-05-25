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
using NETCoreBase.Core.Commands.Users;
using NETCoreBase.Common;
using NETCoreBase.Common.Exceptions;

namespace NETCoreBase.Core.Services
{
    public class UsersService : GenericRepository<User>, IUsersService
    {
        private readonly ILogger<UsersService> _logger;
        private readonly NETCoreBaseContext _context;
        private readonly IMapper _mapper;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly ClaimsPrincipal _clamis;

        public UsersService(ILogger<UsersService> logger, NETCoreBaseContext context, 
            IMapper mapper, IJwtAuthManager jwtAuthManager, ClaimsPrincipal clamis) 
            : base(context, mapper, clamis)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _jwtAuthManager = jwtAuthManager;
            _clamis = clamis;
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<LoginResponse> LoginAsync(LoginRequest req)
        {
            var pass = CryptHelper.HashAu4A83(req.UserPass);
            var user = await base.FirstOrDefaultAsync<User>(
                u => u.UserName == req.UserName && u.PasswordHash == pass && u.Status == "A");

            var log = new UserLogin()
            {
                UserName = req.UserName,
                PasswordHash = req.UserPass,
                Status = (user == null ? "F" : "S"),
                UserId = user?.Id,
                CreateDate = DateTime.Now,
            };
            _context.UserLogins.Add(log);
            await _context.SaveChangesAsync();

            if (user == null)
            {
                throw new MessageException(400, "帳號或密碼錯誤");
            }

            var model = await _context.Users.Where(u => u.Id == user.Id)
                            .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                            .FirstOrDefaultAsync();

            var token = GenerateTokens(model);
            return new LoginResponse() {
                Token = token,
            };
        }

        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest req)
        {
            if (null != await base.FirstOrDefaultAsync<User>(u => u.UserName == req.UserName))
            {
                throw new MessageException(400, "帳號已存在");
            }

            var pass = CryptHelper.HashAu4A83(req.UserPass);
            var user = new User()
            {
                UserName = req.UserName,
                PasswordHash = pass,
                NormalizedUserName = req.NormalizedUserName,
                Email = req.Email,
                PhoneNumber = req.PhoneNumber,
            };

            await base.InsertAsync<User>(user);

            var log = new UserLogin()
            {
                UserName = req.UserName,
                PasswordHash = req.UserPass,
                Status = "S",
                UserId = user?.Id,
                CreateDate = DateTime.Now,
            };
            _context.UserLogins.Add(log);
            await _context.SaveChangesAsync();

            var token = GenerateTokens(user);
            return new RegisterResponse() {
                Token = token,
            };
        }

        private string GenerateTokens(User user)
        {
            var clamiList = new List<Claim> {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            if (user.UserRoles != null && user.UserRoles.Count > 0)
            {
                foreach (var role in user.UserRoles)
                {
                    if (!string.IsNullOrWhiteSpace(role.Role.Name))
                    {
                        clamiList.Add(new Claim(ClaimTypes.Role, role.Role.Name));
                    }
                }
            }

            var token = _jwtAuthManager.GenerateTokens(user.Id.ToString(), clamiList.ToArray(), DateTime.Now);
            return token;
        }

        /// <summary>
        /// 查詢使用者
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<PageResult<UserListResponse>> GetUserListAsync(UserListRequest req)
        {
            var linq = _context.Users.Where(req.GetExpression())
                            .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                            .Select(u => new UserListResponse()
                            {
                                Id = u.Id,
                                UserName = u.UserName,
                                Name = u.NormalizedUserName,
                                Email = u.Email,
                                PhoneNumber = u.PhoneNumber,
                                Status = u.Status,
                                Roles = u.UserRoles.Select(ur => new UserRoleListResponse()
                                {
                                    Id = ur.Role.Id,
                                    Name = ur.Role.Name,
                                    NormalizedName = ur.Role.NormalizedName,
                                }),
                            });
            return new PageResult<UserListResponse>(await linq.ToListAsync(), await linq.CountAsync());
        }

        /// <summary>
        /// 用ID查詢使用者
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<UserByIdResponse> GetUserByIdAsync(UserByIdRequest req)
        {
            var user = await base.FirstOrDefaultAsync<UserByIdResponse>(u => u.Id == req.Id);
            if (user == null)
            {
                throw new MessageException(404, "無此使用者");
            }
            return user;
        }

        /// <summary>
        /// 建立使用者
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task CreateUserAsync(CreateUserRequest req)
        {
            if (null != await base.FirstOrDefaultAsync<User>(u => u.UserName == req.UserName))
            {
                throw new MessageException(400, "帳號已存在");
            }

            var pass = CryptHelper.HashAu4A83(req.UserPass);
            req.UserPass = pass;

            var user = await base.InsertAsync<CreateUserRequest>(req);

            await UpdateUserRoleAsync(user.Id, req.Roles);
        }

        /// <summary>
        /// 修改使用者
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task UpdateUserAsync(UpdateUserRequest req)
        {
            var user = await base.FirstOrDefaultAsync<User>(u => u.Id == req.Id);

            if (user == null)
            {
                throw new MessageException(400, "無此使用者");
            }
            
            user.NormalizedUserName = req.Name;
            user.Email = req.Email;
            user.PhoneNumber = req.PhoneNumber;
            user.Status = req.Status ?? "A";

            await base.UpdateAsync(user);

            await UpdateUserRoleAsync(req.Id, req.Roles);
        }

        /// <summary>
        /// 刪除使用者
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task DeleteUserAsync(DeleteUserRequest req)
        {
            var users = await base.QueryAsync<User>(u => req.Id.Contains(u.Id));
            foreach (var u in users)
            {
                u.Status = "D";
            }
            await base.UpdateRangeAsync(users);
        }

        /// <summary>
        /// 建立使用者擁有的角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        private async Task UpdateUserRoleAsync(Guid userId, List<Guid> roles)
        {
            if (roles != null && roles.Count > 0)
            {
                var list = await _context.UserRoles.Where(u => u.UserId == userId).ToListAsync();

                var adds = roles.Where(r => !list.Any(l => l.RoleId == r)).ToList();
                var removes = list.Where(l => !roles.Any(r => r == l.RoleId)).ToList();
                // var edits = roles.Where(r => !adds.Any(a => a == r)).ToList();

                List<UserRole> urlist = new List<UserRole>();
                foreach (var a in adds)
                {
                    urlist.Add(new UserRole()
                    {
                        UserId = userId,
                        RoleId = a,
                    });
                }
                _context.AddRange(urlist);
                _context.RemoveRange(removes);

                await SaveChangesAsync();
            }
        }
    }
}
