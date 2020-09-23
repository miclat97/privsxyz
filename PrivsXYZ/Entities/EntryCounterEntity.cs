using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivsXYZ.Entities
{
    public class EntryCounterEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string IPv4Address { get; set; }
        public string IPv6Address { get; set; }
        public string Hostname { get; set; }
        public string Site { get; set; }
    }
}
