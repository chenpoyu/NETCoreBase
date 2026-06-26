using System;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NETCoreBase.Core.Commands.Users;

namespace NETCoreBase.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController : BaseApiController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 取得多筆使用者資料
        /// </summary>
        [HttpGet("", Name = nameof(GetAllUsers))]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserListRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 用ID取得使用者資料
        /// </summary>
        [HttpGet("{id}", Name = nameof(GetUserById))]
        public async Task<IActionResult> GetUserById([FromRoute] UserByIdRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 建立使用者
        /// </summary>
        [HttpPost("", Name = nameof(PostUser))]
        public async Task<IActionResult> PostUser(CreateUserRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 修改使用者
        /// </summary>
        [HttpPut("{id}", Name = nameof(PutUser))]
        public async Task<IActionResult> PutUser(Guid id, UpdateUserRequest req)
        {
            if (id != req.Id)
            {
                return BadRequest("Id 錯誤");
            }
            await _mediator.Send(req);
            return NoContent();
        }

        /// <summary>
        /// 刪除使用者
        /// </summary>
        [HttpDelete("", Name = nameof(DeleteUser))]
        public async Task<IActionResult> DeleteUser(DeleteUserRequest req)
        {
            await _mediator.Send(req);
            return NoContent();
        }
    }
}
