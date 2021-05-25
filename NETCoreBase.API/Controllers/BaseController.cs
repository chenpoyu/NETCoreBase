using System.Linq;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace NETCoreBase.API.Controllers
{
    [Authorize]
    public class BaseApiController : ControllerBase
    {
        public readonly string _ip;
        public readonly string _token;
        public BaseApiController()
        {
            IActionContextAccessor accessor = new ActionContextAccessor();
            var ip = accessor.ActionContext.HttpContext.Connection.RemoteIpAddress;
            if (ip != null) {
                if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    ip = System.Net.Dns.GetHostEntry(ip).AddressList.First(c => c.AddressFamily == AddressFamily.InterNetwork);
                }
                _ip = ip.ToString();
            }
            _token = accessor.ActionContext?.HttpContext?.GetTokenAsync("access_token")?.Result;
            //HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}
