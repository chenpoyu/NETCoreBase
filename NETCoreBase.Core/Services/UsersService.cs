using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NETCoreBase.Common;
using NETCoreBase.Common.Exceptions;
using NETCoreBase.Common.Helpers;
using NETCoreBase.Common.Interfaces;
using NETCoreBase.Common.Model;
using NETCoreBase.Common.Resources;
using NETCoreBase.Core.Commands.Users;
using NETCoreBase.Core.Interfaces;
using NETCoreBase.Database;
using NETCoreBase.Database.Models;

namespace NETCoreBase.Core.Services
{
    public class UsersService : IUsersService
    {
        private readonly ILogger<UsersService> _logger;
        private readonly NETCoreBaseContext _context;
        private readonly IGenericRepository<User> _repository;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IPasswordResetStore _resetStore;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public UsersService(
            ILogger<UsersService> logger,
            NETCoreBaseContext context,
            IGenericRepository<User> repository,
            IJwtAuthManager jwtAuthManager,
            IServiceScopeFactory scopeFactory,
            IPasswordResetStore resetStore,
            IStringLocalizer<SharedResource> localizer)
        {
            _logger = logger;
            _context = context;
            _repository = repository;
            _jwtAuthManager = jwtAuthManager;
            _scopeFactory = scopeFactory;
            _resetStore = resetStore;
            _localizer = localizer;
        }

        // Feature 1 + 2: Login with lockout & require-change-password
        public async Task<LoginResponse> LoginAsync(LoginRequest req)
        {
            var user = await _context.Users
                .Where(u => u.UserName == req.UserName && u.Status == "A")
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync();

            // Lockout check
            if (user != null && user.Lockout != null && user.Lockout > DateTimeOffset.Now)
            {
                var mins = (int)Math.Ceiling((user.Lockout.Value - DateTimeOffset.Now).TotalMinutes);
                throw new MessageException(423, string.Format(
                    _localizer["error.user.account_locked_until"].Value, mins));
            }

            var verified = user != null && CryptHelper.VerifyPassword(user.PasswordHash, req.UserPass);

            if (!verified)
            {
                if (user != null)
                {
                    user.AccessFailedCount++;
                    if (user.AccessFailedCount >= 5)
                    {
                        user.Lockout = DateTimeOffset.Now.AddMinutes(30);
                        user.AccessFailedCount = 0;
                    }
                    _context.Entry(user).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                SaveLoginLogAsync(req.UserName, "F", user?.Id);
                throw new MessageException(401, _localizer["error.user.invalid_credentials"].Value);
            }

            // Success — reset lockout
            user.AccessFailedCount = 0;
            user.Lockout = null;
            user.LastLogin = DateTimeOffset.Now;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            SaveLoginLogAsync(req.UserName, "S", user.Id);

            var tokenResult = BuildTokenResult(user);
            return new LoginResponse
            {
                Token = tokenResult.AccessToken,
                RefreshToken = tokenResult.RefreshToken,
                ExpiresAt = tokenResult.ExpiresAt,
                RequireChangePassword = user.RequireChangeMima == "Y",
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest req)
        {
            if (null != await _repository.FirstOrDefaultAsync<User>(u => u.UserName == req.UserName))
                throw new MessageException(400, _localizer["error.user.already_exists"].Value);

            var user = new User
            {
                UserName = req.UserName,
                PasswordHash = CryptHelper.HashPassword(req.UserPass),
                NormalizedUserName = req.NormalizedUserName,
                Email = req.Email,
                PhoneNumber = req.PhoneNumber,
            };
            _context.Users.Add(user);
            await _repository.SaveAsync();
            SaveLoginLogAsync(req.UserName, "S", user.Id);

            var tokenResult = BuildTokenResult(user);
            return new RegisterResponse
            {
                Token = tokenResult.AccessToken,
                RefreshToken = tokenResult.RefreshToken,
                ExpiresAt = tokenResult.ExpiresAt,
            };
        }

        // Feature 2: Change password
        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest req)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Status == "A");
            if (user == null)
                throw new MessageException(404, _localizer["error.user.not_found"].Value);

            if (!CryptHelper.VerifyPassword(user.PasswordHash, req.OldPassword))
                throw new MessageException(400, _localizer["error.password.wrong_old_password"].Value);

            user.PasswordHash = CryptHelper.HashPassword(req.NewPassword);
            user.RequireChangeMima = "N";
            user.LastChangeMimaDate = DateTimeOffset.Now;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Feature 4: Forgot / Reset password
        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest req)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == req.Username && u.Status == "A");
            if (user == null)
                throw new MessageException(404, _localizer["error.user.not_found"].Value);

            var bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);
            var resetToken = Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
            _resetStore.Store(resetToken, req.Username);

            // Skeleton: return token directly (production: send via email)
            return new ForgotPasswordResponse { ResetToken = resetToken };
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest req)
        {
            var username = _resetStore.Consume(req.ResetToken);
            if (username == null)
                throw new MessageException(400, _localizer["error.token.reset_not_found"].Value);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username && u.Status == "A");
            if (user == null)
                throw new MessageException(404, _localizer["error.user.not_found"].Value);

            user.PasswordHash = CryptHelper.HashPassword(req.NewPassword);
            user.RequireChangeMima = "N";
            user.LastChangeMimaDate = DateTimeOffset.Now;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<PageResult<UserListResponse>> GetUserListAsync(UserListRequest req)
        {
            var linq = _context.Users.Where(req.GetExpression())
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Select(u => new UserListResponse
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Name = u.NormalizedUserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Status = u.Status,
                    Roles = u.UserRoles.Select(ur => new UserRoleListResponse
                    {
                        Id = ur.Role.Id,
                        Name = ur.Role.Name,
                        NormalizedName = ur.Role.NormalizedName,
                    }),
                });
            return new PageResult<UserListResponse>(await linq.ToListAsync(), await linq.CountAsync());
        }

        public async Task<UserByIdResponse> GetUserByIdAsync(UserByIdRequest req)
        {
            var user = await _repository.FirstOrDefaultAsync<UserByIdResponse>(u => u.Id == req.Id);
            if (user == null)
                throw new MessageException(404, _localizer["error.user.not_found"].Value);
            return user;
        }

        public async Task CreateUserAsync(CreateUserRequest req)
        {
            if (null != await _repository.FirstOrDefaultAsync<User>(u => u.UserName == req.UserName))
                throw new MessageException(400, _localizer["error.user.already_exists"].Value);

            req.UserPass = CryptHelper.HashPassword(req.UserPass);
            var user = await _repository.InsertAsync<CreateUserRequest>(req);
            await UpdateUserRoleAsync(user.Id, req.Roles);
        }

        public async Task UpdateUserAsync(UpdateUserRequest req)
        {
            var user = await _repository.FirstOrDefaultAsync<User>(u => u.Id == req.Id);
            if (user == null)
                throw new MessageException(400, _localizer["error.user.not_found"].Value);

            user.NormalizedUserName = req.Name;
            user.Email = req.Email;
            user.PhoneNumber = req.PhoneNumber;
            user.Status = req.Status ?? "A";
            await _repository.UpdateAsync(user);
            await UpdateUserRoleAsync(req.Id, req.Roles);
        }

        public async Task DeleteUserAsync(DeleteUserRequest req)
        {
            var users = await _repository.QueryAsync<User>(u => req.Id.Contains(u.Id));
            foreach (var u in users) u.Status = "D";
            await _repository.UpdateRangeAsync(users);
        }

        private TokenResult BuildTokenResult(User user)
        {
            var claimList = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            if (user.UserRoles != null)
            {
                foreach (var ur in user.UserRoles)
                    if (!string.IsNullOrWhiteSpace(ur.Role?.Name))
                        claimList.Add(new Claim(ClaimTypes.Role, ur.Role.Name));
            }
            return _jwtAuthManager.GenerateTokens(user.Id.ToString(), claimList.ToArray(), DateTime.Now);
        }

        // Feature 8: fire-and-forget login log using new DI scope
        private void SaveLoginLogAsync(string userName, string status, Guid? userId)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<NETCoreBaseContext>();
                    context.UserLogins.Add(new UserLogin
                    {
                        UserName = userName,
                        PasswordHash = string.Empty,
                        Status = status,
                        UserId = userId,
                        CreateDate = DateTime.Now,
                    });
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save login log for user {UserName}", userName);
                }
            });
        }

        private async Task UpdateUserRoleAsync(Guid userId, List<Guid> roles)
        {
            if (roles != null && roles.Count > 0)
            {
                var list = await _context.UserRoles.Where(u => u.UserId == userId).ToListAsync();
                var adds = roles.Where(r => !list.Any(l => l.RoleId == r)).ToList();
                var removes = list.Where(l => !roles.Any(r => r == l.RoleId)).ToList();
                var urlist = adds.Select(a => new UserRole { UserId = userId, RoleId = a }).ToList();
                _context.AddRange(urlist);
                _context.RemoveRange(removes);
                await _repository.SaveAsync();
            }
        }
    }
}
