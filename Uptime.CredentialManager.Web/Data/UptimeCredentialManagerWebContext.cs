using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uptime.CredentialManager.Web.Models;

namespace Uptime.CredentialManager.Web.Models
{
    public class UptimeCredentialManagerWebContext : DbContext
    {
        public UptimeCredentialManagerWebContext (DbContextOptions<UptimeCredentialManagerWebContext> options)
            : base(options)
        {
        }

        public DbSet<Uptime.CredentialManager.Web.Models.Credential> Credential { get; set; }

        public DbSet<Uptime.CredentialManager.Web.Models.User> User { get; set; }
    }
}
