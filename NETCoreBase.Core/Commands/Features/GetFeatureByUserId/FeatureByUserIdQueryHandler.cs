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
    public class FeatureByUserIdQueryHandler : IRequestHandler<FeatureByUserIdRequest, FeatureByUserIdResponse>
    {
        private readonly IFeaturesService _featuresService;
        public FeatureByUserIdQueryHandler(IFeaturesService featuresService)
        {
            _featuresService = featuresService;
        }

        public async Task<FeatureByUserIdResponse> Handle(FeatureByUserIdRequest request, CancellationToken cancellationToken)
        {
            return await _featuresService.GetFeatureByUserIdAsync(request);
        }
    }
}