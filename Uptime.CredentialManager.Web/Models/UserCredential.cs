using System;

namespace Uptime.CredentialManager.Web.Models
{
    public class UserCredential
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid CredentialId { get; set; }
        public Credential Credential { get; set; }

       
    }
}
