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
    public class FeatureByIdQueryHandler : IRequestHandler<FeatureByIdRequest, FeatureByIdResponse>
    {
        private readonly IFeaturesService _featuresService;
        public FeatureByIdQueryHandler(IFeaturesService featuresService)
        {
            _featuresService = featuresService;
        }

        public async Task<FeatureByIdResponse> Handle(FeatureByIdRequest request, CancellationToken cancellationToken)
        {
            return await _featuresService.GetFeatureByIdAsync(request);
        }
    }
}