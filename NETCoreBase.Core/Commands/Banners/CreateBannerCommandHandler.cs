using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Banners
{
    public class CreateBannerCommandHandler : IRequestHandler<CreateBannerRequest>
    {
        private readonly IBannersService _bannersService;

        public CreateBannerCommandHandler(IBannersService bannersService)
        {
            _bannersService = bannersService;
        }

        public async Task Handle(CreateBannerRequest request, CancellationToken cancellationToken)
        {
            await _bannersService.CreateBannerAsync(request);
        }
    }
}
