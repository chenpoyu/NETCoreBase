using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NETCoreBase.Common.Handlers;
using NETCoreBase.Common.Policies;
using NETCoreBase.Core.Commands.Users;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("", Name = nameof(GetAllUsers))]
        //[Permission(PermissionPolicy.Query)]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserListRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 取得多筆使用者資料
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetUserById))]
        //[Permission(PermissionPolicy.Query)]
        public async Task<IActionResult> GetUserById([FromRoute] UserByIdRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 建立使用者
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("", Name = nameof(PostUser))]
        public async Task<IActionResult> PostUser(CreateUserRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 修改使用者
        /// </summary>
        /// <param name="id"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        // [Authorize(Roles = "CodeSet")]
        [HttpPut("{id}", Name = nameof(PutUser))]
        //[Permission(PermissionPolicy.Update)]
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
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("", Name = nameof(DeleteUser))]
        public async Task<IActionResult> DeleteUser(DeleteUserRequest req)
        {
            await _mediator.Send(req);
            return NoContent();
        }

    }
}
