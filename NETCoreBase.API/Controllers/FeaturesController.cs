using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NETCoreBase.Common.Handlers;
using NETCoreBase.Common.Policies;
using NETCoreBase.Core.Commands.Features;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeaturesController : BaseApiController
    {
        private readonly IMediator _mediator;
        
        public FeaturesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 取得多筆功能資料
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("", Name = nameof(GetAllFeatures))]
        public async Task<IActionResult> GetAllFeatures([FromQuery] FeatureListRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 取得多筆功能資料
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetFeatureById))]
        public async Task<IActionResult> GetFeatureById([FromRoute] FeatureByIdRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 建立功能
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("", Name = nameof(PostFeature))]
        public async Task<IActionResult> PostFeature(CreateFeatureRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 修改功能
        /// </summary>
        /// <param name="id"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPut("{id}", Name = nameof(PutFeature))]
        public async Task<IActionResult> PutFeature(Guid id, UpdateFeatureRequest req)
        {
            if (id != req.Id)
            {
                return BadRequest("Id 錯誤");
            }
            await _mediator.Send(req);
            return NoContent();
        }

        /// <summary>
        /// 刪除功能
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("", Name = nameof(DeleteFeature))]
        public async Task<IActionResult> DeleteFeature(DeleteFeatureRequest req)
        {
            await _mediator.Send(req);
            return NoContent();
        }

    }
}
