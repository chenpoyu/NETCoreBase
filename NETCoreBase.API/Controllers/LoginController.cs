using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NETCoreBase.Core.Commands.Features;
using NETCoreBase.Core.Commands.Users;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : BaseApiController
    {
        private readonly IMediator _mediator;
        
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
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
        /// 取得功能樹
        /// </summary>
        [HttpGet("", Name = nameof(GetFeatureTree))]
        public async Task<IActionResult> GetFeatureTree()
        {
            var claim = User.FindFirst("userId");
            var req = new FeatureByUserIdRequest()
            {
                Id = Guid.Parse(claim?.Value)
            };
            return Ok(await _mediator.Send(req));
        }
    }
}
