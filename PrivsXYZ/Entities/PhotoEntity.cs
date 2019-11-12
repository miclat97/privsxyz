using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivsXYZ.Entities
{
    public class PhotoEntity
    {
        [Key]
        public int Id { get; set; }
        public string PhotoIdentityString { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string IPv4Address { get; set; }
        public string IPv6Address { get; set; }
        public string Hostname { get; set; }
        public byte[] Image { get; set; }
        public string ViewerIPv4 { get; set; }
        public string ViewerIPv6 { get; set; }
        public string ViewerHostname { get; set; }
        public DateTime OpenedDate { get; set; }
    }
}
