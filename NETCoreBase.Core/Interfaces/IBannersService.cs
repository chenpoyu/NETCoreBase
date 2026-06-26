using System;
using System.Threading.Tasks;
using NETCoreBase.Common;
using NETCoreBase.Core.Commands.Banners;

namespace NETCoreBase.Core.Interfaces
{
    public interface IBannersService : IDisposable
    {
        Task<PageResult<BannerListResponse>> GetBannerListAsync(BannerListRequest req);
        Task<BannerByIdResponse> GetBannerByIdAsync(BannerByIdRequest req);
        Task CreateBannerAsync(CreateBannerRequest req);
        Task UpdateBannerAsync(UpdateBannerRequest req);
        Task DeleteBannerAsync(DeleteBannerRequest req);
    }
}
