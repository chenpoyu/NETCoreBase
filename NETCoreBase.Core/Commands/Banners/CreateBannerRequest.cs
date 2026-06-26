using MediatR;

namespace NETCoreBase.Core.Commands.Banners
{
    public class CreateBannerRequest : IRequest
    {
        /// <summary>
        /// 圖片路徑
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 狀態（A：上架、U：未上架）
        /// </summary>
        public string Enable { get; set; } = "U";
    }
}
