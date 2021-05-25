using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NETCoreBase.Common.Handlers;
using NETCoreBase.Common.Policies;
using NETCoreBase.Core.Commands.Roles;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : BaseApiController
    {
        private readonly IMediator _mediator;
        
        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 取得多筆角色資料
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("", Name = nameof(GetAllRoles))]
        public async Task<IActionResult> GetAllRoles([FromQuery] RoleListRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 取得多筆角色資料
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetRoleById))]
        public async Task<IActionResult> GetRoleById([FromRoute] RoleByIdRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 建立角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("", Name = nameof(PostRole))]
        public async Task<IActionResult> PostRole(CreateRoleRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPut("{id}", Name = nameof(PutRole))]
        public async Task<IActionResult> PutRole(Guid id, UpdateRoleRequest req)
        {
            if (id != req.Id)
            {
                return BadRequest("Id 錯誤");
            }
            await _mediator.Send(req);
            return NoContent();
        }

        /// <summary>
        /// 刪除角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("", Name = nameof(DeleteRole))]
        public async Task<IActionResult> DeleteRole(DeleteRoleRequest req)
        {
            await _mediator.Send(req);
            return NoContent();
        }

    }
}
