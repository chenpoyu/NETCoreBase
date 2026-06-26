using System;
using MediatR;

namespace NETCoreBase.Core.Commands.Banners
{
    public class DeleteBannerRequest : IRequest
    {
        public Guid Id { get; set; }
    }
}
