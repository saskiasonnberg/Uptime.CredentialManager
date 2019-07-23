using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uptime.CredentialManager.Web.ViewModels
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public Guid UserId { get; set; }

        public IEnumerable <CredentialViewModel> CredentialList { get; set; }
    }
}
