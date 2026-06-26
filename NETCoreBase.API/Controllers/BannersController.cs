using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NETCoreBase.Core.Commands.Banners;

namespace NETCoreBase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BannersController : BaseApiController
    {
        private readonly IMediator _mediator;

        public BannersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 取得 Banner 列表（分頁）
        /// </summary>
        [HttpGet("", Name = nameof(GetAllBanners))]
        public async Task<IActionResult> GetAllBanners([FromQuery] BannerListRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 取得單筆 Banner
        /// </summary>
        [HttpGet("{id}", Name = nameof(GetBannerById))]
        public async Task<IActionResult> GetBannerById([FromRoute] BannerByIdRequest req)
        {
            return Ok(await _mediator.Send(req));
        }

        /// <summary>
        /// 建立 Banner
        /// </summary>
        [HttpPost("", Name = nameof(PostBanner))]
        public async Task<IActionResult> PostBanner(CreateBannerRequest req)
        {
            await _mediator.Send(req);
            return Ok();
        }

        /// <summary>
        /// 修改 Banner
        /// </summary>
        [HttpPut("{id}", Name = nameof(PutBanner))]
        public async Task<IActionResult> PutBanner(Guid id, UpdateBannerRequest req)
        {
            if (id != req.Id)
                return BadRequest("Id 錯誤");
            await _mediator.Send(req);
            return NoContent();
        }

        /// <summary>
        /// 軟刪除 Banner（Enable = 'D'）
        /// </summary>
        [HttpDelete("{id}", Name = nameof(DeleteBanner))]
        public async Task<IActionResult> DeleteBanner([FromRoute] DeleteBannerRequest req)
        {
            await _mediator.Send(req);
            return NoContent();
        }
    }
}
