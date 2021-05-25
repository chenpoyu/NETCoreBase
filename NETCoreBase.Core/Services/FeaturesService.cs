using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NETCoreBase.Core.Commands;
using NETCoreBase.Core.Interfaces;
using NETCoreBase.Common.Interfaces;
using NETCoreBase.Common.Services;
using NETCoreBase.Database;
using NETCoreBase.Database.Models;
using NETCoreBase.Common.Helpers;
using NETCoreBase.Core.Commands.Features;
using NETCoreBase.Common;
using NETCoreBase.Common.Exceptions;

namespace NETCoreBase.Core.Services
{
    public class FeaturesService : GenericRepository<Feature>, IFeaturesService
    {
        private readonly ILogger<FeaturesService> _logger;
        private readonly NETCoreBaseContext _context;
        private readonly IMapper _mapper;
        private readonly ClaimsPrincipal _clamis;

        public FeaturesService(ILogger<FeaturesService> logger, NETCoreBaseContext context, 
            IMapper mapper, ClaimsPrincipal clamis) 
            : base(context, mapper, clamis)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _clamis = clamis;
        }

        /// <summary>
        /// 查詢功能
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<PageResult<FeatureListResponse>> GetFeatureListAsync(FeatureListRequest req)
        {
            return await base.QueryAsync<FeatureListResponse>(req.GetExpression(), req);
        }

        /// <summary>
        /// 用ID查詢功能
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<FeatureByIdResponse> GetFeatureByIdAsync(FeatureByIdRequest req)
        {
            var feature = await base.FirstOrDefaultAsync<FeatureByIdResponse>(u => u.Id == req.Id);
            if (feature == null)
            {
                throw new MessageException(404, "無此功能");
            }
            return feature;
        }

        /// <summary>
        /// 用ID查詢功能
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<FeatureByUserIdResponse> GetFeatureByUserIdAsync(FeatureByUserIdRequest req)
        {
            var feature = await base.FirstOrDefaultAsync<FeatureByUserIdResponse>(u => u.Id == req.Id);
            if (feature == null)
            {
                throw new MessageException(404, "無此功能");
            }
            return feature;
        }

        /// <summary>
        /// 建立功能
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task CreateFeatureAsync(CreateFeatureRequest req)
        {
            await base.InsertAsync<CreateFeatureRequest>(req);
        }

        /// <summary>
        /// 修改功能
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task UpdateFeatureAsync(UpdateFeatureRequest req)
        {
            var feature = await base.FirstOrDefaultAsync<Feature>(u => u.Id == req.Id);

            if (feature == null)
            {
                throw new MessageException(400, "無此功能");
            }
            
            feature.ParentId = req.ParentId;
            feature.Seq = req.Seq;
            feature.Name = req.Name;
            feature.Path = req.Path;
            feature.Status = req.Status ?? "A";
            
            await base.UpdateAsync(feature);
        }

        /// <summary>
        /// 刪除功能
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task DeleteFeatureAsync(DeleteFeatureRequest req)
        {
            var features = await base.QueryAsync<Feature>(u => req.Id.Contains(u.Id));
            foreach (var u in features)
            {
                u.Status = "D";
            }
            await base.UpdateRangeAsync(features);
        }
    }
}
