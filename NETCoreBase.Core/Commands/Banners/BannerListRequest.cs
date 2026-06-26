using System.Collections.Generic;
using MediatR;
using NETCoreBase.Common;

namespace NETCoreBase.Core.Commands.Banners
{
    public class BannerListRequest : QueryOption, IRequest<PageResult<BannerListResponse>>
    {
        /// <summary>
        /// 篩選啟用狀態（A/U/D），不傳則查全部非刪除
        /// </summary>
        public string Enable { get; set; }
    }
}
