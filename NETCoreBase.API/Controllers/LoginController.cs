using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NETCoreBase.Core.Commands.Features;
using NETCoreBase.Core.Commands.Users;

namespace NETCoreBase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 登入
        /// </summary>
        [AllowAnonymous]
        [HttpPost("", Name = nameof(Login))]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 註冊
        /// </summary>
        [AllowAnonymous]
        [HttpPost("", Name = nameof(Register))]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 登出（將 Token 加入黑名單）
        /// </summary>
        [HttpPost("", Name = nameof(Logout))]
        public async Task<IActionResult> Logout()
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString() ?? "";
            var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authHeader["Bearer ".Length..].Trim()
                : null;

            var userId = User.FindFirst("userId")?.Value;

            await _mediator.Send(new LogoutRequest { Token = token, UserId = userId });
            return Ok();
        }

        /// <summary>
        /// 變更密碼（需先登入）
        /// </summary>
        [HttpPost("", Name = nameof(ChangePassword))]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest req)
        {
            await _mediator.Send(req);
            return Ok();
        }

        /// <summary>
        /// 忘記密碼（骨架版：直接回傳 resetToken）
        /// </summary>
        [AllowAnonymous]
        [HttpPost("", Name = nameof(ForgotPassword))]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 重設密碼
        /// </summary>
        [AllowAnonymous]
        [HttpPost("", Name = nameof(ResetPassword))]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest req)
        {
            await _mediator.Send(req);
            return Ok();
        }

        /// <summary>
        /// 取得功能樹
        /// </summary>
        [HttpGet("", Name = nameof(GetFeatureTree))]
        public async Task<IActionResult> GetFeatureTree()
        {
            var claim = User.FindFirst("userId");
            var req = new FeatureByUserIdRequest
            {
                Id = Guid.Parse(claim?.Value)
            };
            return Ok(await _mediator.Send(req));
        }
    }
}
