using System;
using System.Collections.Generic;

namespace Uptime.CredentialManager.Web.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }    
        
        public string Role { get; set; }

        public ICollection<UserCredential> UserCredentials { get; set; }

    }
}
