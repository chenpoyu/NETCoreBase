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
    public class DeleteFeatureCommandHandler : IRequestHandler<DeleteFeatureRequest, Unit>
    {
        private readonly IFeaturesService _featureService;
        public DeleteFeatureCommandHandler(IFeaturesService featureService)
        {
            _featureService = featureService;
        }

        public async Task<Unit> Handle(DeleteFeatureRequest request, CancellationToken cancellationToken)
        {
            await _featureService.DeleteFeatureAsync(request);
            return Unit.Value;
        }
    }
}
