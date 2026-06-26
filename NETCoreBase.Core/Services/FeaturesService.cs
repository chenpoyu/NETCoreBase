using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NETCoreBase.Core.Interfaces;
using NETCoreBase.Common.Interfaces;
using NETCoreBase.Database.Models;
using NETCoreBase.Core.Commands.Features;
using NETCoreBase.Common;
using NETCoreBase.Common.Exceptions;

namespace NETCoreBase.Core.Services
{
    public class FeaturesService : IFeaturesService
    {
        private readonly ILogger<FeaturesService> _logger;
        private readonly IGenericRepository<Feature> _repository;

        public FeaturesService(ILogger<FeaturesService> logger, IGenericRepository<Feature> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<PageResult<FeatureListResponse>> GetFeatureListAsync(FeatureListRequest req)
        {
            return await _repository.QueryAsync<FeatureListResponse>(req.GetExpression(), req);
        }

        public async Task<FeatureByIdResponse> GetFeatureByIdAsync(FeatureByIdRequest req)
        {
            var feature = await _repository.FirstOrDefaultAsync<FeatureByIdResponse>(u => u.Id == req.Id);
            if (feature == null)
            {
                throw new MessageException(404, "無此功能");
            }
            return feature;
        }

        public async Task<FeatureByUserIdResponse> GetFeatureByUserIdAsync(FeatureByUserIdRequest req)
        {
            var feature = await _repository.FirstOrDefaultAsync<FeatureByUserIdResponse>(u => u.Id == req.Id);
            if (feature == null)
            {
                throw new MessageException(404, "無此功能");
            }
            return feature;
        }

        public async Task CreateFeatureAsync(CreateFeatureRequest req)
        {
            await _repository.InsertAsync<CreateFeatureRequest>(req);
        }

        public async Task UpdateFeatureAsync(UpdateFeatureRequest req)
        {
            var feature = await _repository.FirstOrDefaultAsync<Feature>(u => u.Id == req.Id);

            if (feature == null)
            {
                throw new MessageException(400, "無此功能");
            }

            feature.ParentId = req.ParentId;
            feature.Seq = req.Seq;
            feature.Name = req.Name;
            feature.Path = req.Path;
            feature.Status = req.Status ?? "A";

            await _repository.UpdateAsync(feature);
        }

        public async Task DeleteFeatureAsync(DeleteFeatureRequest req)
        {
            var features = await _repository.QueryAsync<Feature>(u => req.Id.Contains(u.Id));
            foreach (var u in features)
            {
                u.Status = "D";
            }
            await _repository.UpdateRangeAsync(features);
        }
    }
}
