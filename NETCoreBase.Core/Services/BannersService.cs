using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NETCoreBase.Common;
using NETCoreBase.Common.Exceptions;
using NETCoreBase.Common.Services;
using NETCoreBase.Core.Commands.Banners;
using NETCoreBase.Core.Interfaces;
using NETCoreBase.Database;
using NETCoreBase.Database.Models;

namespace NETCoreBase.Core.Services
{
    public class BannersService : GenericRepository<Banner>, IBannersService
    {
        private readonly ILogger<BannersService> _logger;
        private readonly NETCoreBaseContext _context;
        private readonly IMapper _mapper;

        public BannersService(ILogger<BannersService> logger, NETCoreBaseContext context,
            IMapper mapper, ClaimsPrincipal claims)
            : base(context, mapper, claims)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<PageResult<BannerListResponse>> GetBannerListAsync(BannerListRequest req)
        {
            var query = _context.Banners
                .Where(b => b.Enable != "D");

            if (!string.IsNullOrWhiteSpace(req.Enable))
                query = query.Where(b => b.Enable == req.Enable);

            var total = await query.CountAsync();
            var pagedQuery = req.ApplyTo(query.OrderByDescending(b => b.CreateDate));
            var items = await pagedQuery
                .Select(b => new BannerListResponse
                {
                    Id = b.Id,
                    Image = b.Image,
                    Enable = b.Enable,
                    CreateDate = b.CreateDate,
                    UpdateDate = b.UpdateDate,
                })
                .ToListAsync();

            return new PageResult<BannerListResponse>(items, total);
        }

        public async Task<BannerByIdResponse> GetBannerByIdAsync(BannerByIdRequest req)
        {
            var banner = await _context.Banners
                .Where(b => b.Id == req.Id && b.Enable != "D")
                .FirstOrDefaultAsync();

            if (banner == null)
                throw new MessageException(404, "查無此 Banner");

            return new BannerByIdResponse
            {
                Id = banner.Id,
                Image = banner.Image,
                Enable = banner.Enable,
                CreateDate = banner.CreateDate,
                UpdateDate = banner.UpdateDate,
                Version = banner.Version,
            };
        }

        public async Task CreateBannerAsync(CreateBannerRequest req)
        {
            var banner = new Banner
            {
                Image = req.Image,
                Enable = req.Enable ?? "U",
                CreateDate = DateTimeOffset.Now,
                UpdateDate = DateTimeOffset.Now,
            };
            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBannerAsync(UpdateBannerRequest req)
        {
            var banner = await _context.Banners
                .Where(b => b.Id == req.Id && b.Enable != "D")
                .FirstOrDefaultAsync();

            if (banner == null)
                throw new MessageException(404, "查無此 Banner");

            banner.Image = req.Image;
            banner.Enable = req.Enable ?? banner.Enable;
            banner.Version = req.Version; // EF will check concurrency token

            await base.UpdateAsync(banner);
        }

        public async Task DeleteBannerAsync(DeleteBannerRequest req)
        {
            var banner = await _context.Banners
                .Where(b => b.Id == req.Id)
                .FirstOrDefaultAsync();

            if (banner == null)
                throw new MessageException(404, "查無此 Banner");

            banner.Enable = "D";
            await base.UpdateAsync(banner);
        }
    }
}
