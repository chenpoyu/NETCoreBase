using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Banners
{
    public class BannerByIdQueryHandler : IRequestHandler<BannerByIdRequest, BannerByIdResponse>
    {
        private readonly IBannersService _bannersService;

        public BannerByIdQueryHandler(IBannersService bannersService)
        {
            _bannersService = bannersService;
        }

        public async Task<BannerByIdResponse> Handle(BannerByIdRequest request, CancellationToken cancellationToken)
        {
            return await _bannersService.GetBannerByIdAsync(request);
        }
    }
}
