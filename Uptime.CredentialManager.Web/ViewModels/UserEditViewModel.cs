﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Uptime.CredentialManager.Web.ViewModels
{
    public class UserEditViewModel : UserViewModel
    {       
        public IEnumerable<Guid> SelectedCredential { get; set; }
        public IEnumerable<SelectListItem> Credentials { get; set; }

    }
}
