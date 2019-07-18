using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uptime.CredentialManager.Web.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<UserCredential> UserCredentials { get; set; }

    }
}
