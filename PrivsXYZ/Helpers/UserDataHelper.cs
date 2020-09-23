using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace PrivsXYZ.Helpers
{
    public class UserDataHelper : IUserDataHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserDataHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Tuple<string, string, string> GetUserData()
        {
            var ipAddressv4 = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4();
            var ipAddressv6 = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv6();

            string ipV4InString = ipAddressv4?.ToString();
            string ipV6InString = ipAddressv6?.ToString();

            string hostname = ipAddressv4?.ToString();

            try
            {
                var hostEntry = Dns.GetHostEntry(ipAddressv4?.ToString())?.HostName;
                hostname = hostEntry;
            }
            catch
            {
                
            }

            return Tuple.Create(ipV4InString, ipV6InString, hostname);
        }
    }

}