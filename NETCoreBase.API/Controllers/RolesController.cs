using System;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NETCoreBase.Core.Commands.Roles;

namespace NETCoreBase.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
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
        [HttpGet("", Name = nameof(GetAllRoles))]
        public async Task<IActionResult> GetAllRoles([FromQuery] RoleListRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 用ID取得角色資料
        /// </summary>
        [HttpGet("{id}", Name = nameof(GetRoleById))]
        public async Task<IActionResult> GetRoleById([FromRoute] RoleByIdRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 建立角色
        /// </summary>
        [HttpPost("", Name = nameof(PostRole))]
        public async Task<IActionResult> PostRole(CreateRoleRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 修改角色
        /// </summary>
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
        [HttpDelete("", Name = nameof(DeleteRole))]
        public async Task<IActionResult> DeleteRole(DeleteRoleRequest req)
        {
            await _mediator.Send(req);
            return NoContent();
        }
    }
}
