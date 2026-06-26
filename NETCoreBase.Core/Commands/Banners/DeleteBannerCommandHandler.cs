using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Banners
{
    public class DeleteBannerCommandHandler : IRequestHandler<DeleteBannerRequest>
    {
        private readonly IBannersService _bannersService;

        public DeleteBannerCommandHandler(IBannersService bannersService)
        {
            _bannersService = bannersService;
        }

        public async Task Handle(DeleteBannerRequest request, CancellationToken cancellationToken)
        {
            await _bannersService.DeleteBannerAsync(request);
        }
    }
}
