using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Common;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Banners
{
    public class BannerListQueryHandler : IRequestHandler<BannerListRequest, PageResult<BannerListResponse>>
    {
        private readonly IBannersService _bannersService;

        public BannerListQueryHandler(IBannersService bannersService)
        {
            _bannersService = bannersService;
        }

        public async Task<PageResult<BannerListResponse>> Handle(BannerListRequest request, CancellationToken cancellationToken)
        {
            return await _bannersService.GetBannerListAsync(request);
        }
    }
}
