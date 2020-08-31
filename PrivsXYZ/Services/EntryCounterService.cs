using PrivsXYZ.Data;
using PrivsXYZ.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivsXYZ.Services
{
    public class EntryCounterService : IEntryCounterService
    {
        private readonly PrivsDbContext _context;

        public EntryCounterService(PrivsDbContext context)
        {
            _context = context;
        }

        public async Task RegisterSiteEnter(string ipv4, string ipv6, string hostname)
        {
            var newEntry = new EntryCounterEntity()
            {
                DateTime = DateTime.Now,
                Hostname = hostname,
                IPv4Address = ipv4,
                IPv6Address = ipv6
            };

            await _context.EntryCounter.AddAsync(newEntry);

            await _context.SaveChangesAsync();
        }
    }
}
