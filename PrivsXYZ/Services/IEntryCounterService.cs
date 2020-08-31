using PrivsXYZ.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivsXYZ.Services
{
    public interface IEntryCounterService
    {
        Task RegisterSiteEnter(string ipv4, string ipv6, string hostname);
    }
}
