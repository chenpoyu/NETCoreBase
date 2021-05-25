using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Core.Commands;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Features
{
    public class UpdateFeatureCommandHandler : IRequestHandler<UpdateFeatureRequest, Unit>
    {
        private readonly IFeaturesService _featuresService;
        public UpdateFeatureCommandHandler(IFeaturesService featuresService)
        {
            _featuresService = featuresService;
        }

        public async Task<Unit> Handle(UpdateFeatureRequest request, CancellationToken cancellationToken)
        {
            await _featuresService.UpdateFeatureAsync(request);
            return Unit.Value;
        }

    }
}
