using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Banners
{
    public class UpdateBannerCommandHandler : IRequestHandler<UpdateBannerRequest>
    {
        private readonly IBannersService _bannersService;

        public UpdateBannerCommandHandler(IBannersService bannersService)
        {
            _bannersService = bannersService;
        }

        public async Task Handle(UpdateBannerRequest request, CancellationToken cancellationToken)
        {
            await _bannersService.UpdateBannerAsync(request);
        }
    }
}
