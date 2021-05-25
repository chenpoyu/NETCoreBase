using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NETCoreBase.Core.Interfaces;

namespace NETCoreBase.Core.Commands.Features
{
    public class CreateFeatureCommandHandler : IRequestHandler<CreateFeatureRequest, Unit>
    {
        private readonly IFeaturesService _featureService;
        public CreateFeatureCommandHandler(IFeaturesService featureService)
        {
            _featureService = featureService;
        }

        public async Task<Unit> Handle(CreateFeatureRequest request, CancellationToken cancellationToken)
        {
            await _featureService.CreateFeatureAsync(request);
            return Unit.Value;
        }
    }
}
