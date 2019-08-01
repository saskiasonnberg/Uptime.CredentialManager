using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Uptime.CredentialManager.Web.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public string UserRole { get; set; }

        public IEnumerable <CredentialViewModel> CredentialList { get; set; }
    }
}
