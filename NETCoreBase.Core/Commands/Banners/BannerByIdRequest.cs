using System;
using MediatR;

namespace NETCoreBase.Core.Commands.Banners
{
    public class BannerByIdRequest : IRequest<BannerByIdResponse>
    {
        public Guid Id { get; set; }
    }
}
