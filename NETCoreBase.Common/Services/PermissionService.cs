using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NETCoreBase.Common.Interfaces;
using NETCoreBase.Common.Model;
using NETCoreBase.Common.Services;
using NETCoreBase.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
// using NETCoreBaseContextPartial =  NETCoreBase.Database.Partial.NETCoreBaseContext;

namespace NETCoreBase.Common.Services
{
    public class PermissionService : GenericRepository<FeaturePermission>, IPermissionService
    {
        private readonly ILogger<PermissionService> _logger;
        private readonly NETCoreBaseContext _context;
        private readonly IMemoryCache _cache;

        public PermissionService(ILogger<PermissionService> logger, NETCoreBaseContext context, IMemoryCache cache) : base(context)
        {
            _logger = logger;
            _context = context;
            _cache = cache;
        }

        public async Task<bool> HasPermission(ClaimsPrincipal user, List<string> permissions)
        {
            if (!_cache.TryGetValue<Dictionary<string, List<RoleFeaturePermission>>>("permissionDic", out var permissionDic))
            {
                var linq = from r in _context.Roles
                           join rf in _context.RoleFeatures on r.Id equals rf.RoleId
                           join fp in _context.FeaturePermissions on rf.FeatureId equals fp.FeatureId
                           select new RoleFeaturePermission
                           {
                               RoleName = r.Name,
                               FeatureId = rf.FeatureId.ToString(),
                               Permission = fp.Permission
                           };
                var list = await linq.ToListAsync();
                permissionDic = list.GroupBy(v => v.Permission).ToDictionary(g => g.Key, g => g.ToList());
                _cache.Set("permissionDic", permissionDic, TimeSpan.FromMinutes(5));
            }

            var hasPermission = false;
            foreach (var permission in permissions)
            {
                if (permissionDic.TryGetValue(permission, out var g))
                {
                    if (g.Any(v => user.IsInRole(v.RoleName) || user.HasClaim("featureId", v.FeatureId)))
                    {
                        hasPermission = true;
                        break;
                    }
                }
            }

            return hasPermission;
        }
    }
}
