using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uptime.CredentialManager.Web.Models
{
    public class UserCredential
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid CredentialId { get; set; }
        public Credential Credential { get; set; }



    }
}
