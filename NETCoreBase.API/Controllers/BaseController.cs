using System.Linq;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NETCoreBase.API.Controllers
{
    [Authorize]
    public class BaseApiController : ControllerBase
    {
        protected string ClientIp
        {
            get
            {
                var ip = HttpContext?.Connection?.RemoteIpAddress;
                if (ip == null) return null;
                if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    var ipv4 = System.Net.Dns.GetHostEntry(ip).AddressList
                        .FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);
                    return ipv4?.ToString();
                }
                return ip.ToString();
            }
        }

        protected string BearerToken
        {
            get
            {
                var authHeader = HttpContext?.Request?.Headers?.Authorization.ToString();
                return authHeader?.StartsWith("Bearer ") == true ? authHeader[7..] : null;
            }
        }
    }
}
