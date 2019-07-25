using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Uptime.CredentialManager.Web.ViewModels
{
    public class CredentialEditViewModel : CredentialViewModel
    {
        public IEnumerable<Guid> SelectedUser { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; }
    }
}
