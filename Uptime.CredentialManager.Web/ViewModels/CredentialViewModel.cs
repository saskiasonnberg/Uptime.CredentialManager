using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uptime.CredentialManager.Web.ViewModels
{
    public class CredentialViewModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }

        public IEnumerable<UserViewModel> UserList { get; set; }
    }
}
