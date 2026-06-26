using System;

namespace NETCoreBase.Core.Commands.Banners
{
    public class BannerByIdResponse
    {
        public Guid Id { get; set; }
        public string Image { get; set; }
        public string Enable { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
        public long Version { get; set; }
    }
}
