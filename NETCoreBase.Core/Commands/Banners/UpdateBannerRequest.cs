using System;
using MediatR;

namespace NETCoreBase.Core.Commands.Banners
{
    public class UpdateBannerRequest : IRequest
    {
        public Guid Id { get; set; }
        public string Image { get; set; }
        public string Enable { get; set; }
        public long Version { get; set; }
    }
}
