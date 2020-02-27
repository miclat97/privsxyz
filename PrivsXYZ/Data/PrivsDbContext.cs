using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PrivsXYZ.Entities;

namespace PrivsXYZ.Data
{
    public class PrivsDbContext : IdentityDbContext
    {
        public PrivsDbContext(DbContextOptions<PrivsDbContext> options)
            : base(options)
        {
        }

        public DbSet<MessageEntity> Message { get; set; }
        public DbSet<PhotoEntity> Photo { get; set; }
        public DbSet<FileEntity> File { get; set; }
    }
}
