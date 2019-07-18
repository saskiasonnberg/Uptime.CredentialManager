using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uptime.CredentialManager.Web.Models
{
    public class Credential
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }

        public ICollection<UserCredential> UserCredentials { get; set; }
    }
}
