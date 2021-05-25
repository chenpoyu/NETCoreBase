using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NETCoreBase.Core.Commands.Features;
using NETCoreBase.Common.Interfaces;
using MediatR;
using NETCoreBase.Common;

namespace NETCoreBase.Core.Interfaces
{
    public interface IFeaturesService : IDisposable
    {
        /// <summary>
        /// 查詢功能
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<PageResult<FeatureListResponse>> GetFeatureListAsync(FeatureListRequest req);

        /// <summary>
        /// 用ID查詢功能
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<FeatureByIdResponse> GetFeatureByIdAsync(FeatureByIdRequest req);
        public Task<FeatureByUserIdResponse> GetFeatureByUserIdAsync(FeatureByUserIdRequest req);

        /// <summary>
        /// 建立功能
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task CreateFeatureAsync(CreateFeatureRequest req);

        /// <summary>
        /// 修改功能
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task UpdateFeatureAsync(UpdateFeatureRequest req);

        /// <summary>
        /// 刪除功能
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task DeleteFeatureAsync(DeleteFeatureRequest req);
    }
}


