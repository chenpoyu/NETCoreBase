using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NETCoreBase.Common.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> HasPermission(ClaimsPrincipal user, List<string> permissions);
        void ClearPermissionCache();
    }
}
