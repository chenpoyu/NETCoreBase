using System;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NETCoreBase.Core.Commands.Features;

namespace NETCoreBase.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
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
        [HttpGet("", Name = nameof(GetAllFeatures))]
        public async Task<IActionResult> GetAllFeatures([FromQuery] FeatureListRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 用ID取得功能資料
        /// </summary>
        [HttpGet("{id}", Name = nameof(GetFeatureById))]
        public async Task<IActionResult> GetFeatureById([FromRoute] FeatureByIdRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 建立功能
        /// </summary>
        [HttpPost("", Name = nameof(PostFeature))]
        public async Task<IActionResult> PostFeature(CreateFeatureRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 修改功能
        /// </summary>
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
        [HttpDelete("", Name = nameof(DeleteFeature))]
        public async Task<IActionResult> DeleteFeature(DeleteFeatureRequest req)
        {
            await _mediator.Send(req);
            return NoContent();
        }
    }
}
