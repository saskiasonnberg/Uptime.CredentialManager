using System;
using System.Collections.Generic;

namespace Uptime.CredentialManager.Web.Models
{
    public class Credential
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Description { get; set; }

        public string Value { get; set; }

        public ICollection<UserCredential> UserCredentials { get; set; }
    }
}
