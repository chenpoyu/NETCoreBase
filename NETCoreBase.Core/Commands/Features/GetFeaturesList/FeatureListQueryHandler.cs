using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Common;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Features
{
    public class FeatureListQueryHandler : IRequestHandler<FeatureListRequest, PageResult<FeatureListResponse>>
    {
        private readonly IFeaturesService _featuresService;
        public FeatureListQueryHandler(IFeaturesService featuresService)
        {
            _featuresService = featuresService;
        }

        public async Task<PageResult<FeatureListResponse>> Handle(FeatureListRequest request, CancellationToken cancellationToken)
        {
            return await _featuresService.GetFeatureListAsync(request);
        }
    }
}