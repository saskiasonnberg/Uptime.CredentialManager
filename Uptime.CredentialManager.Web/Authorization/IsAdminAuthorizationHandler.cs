using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Uptime.CredentialManager.Web.Models;

namespace Uptime.CredentialManager.Web.Authorization
{
    public class IsAdminAuthorizationHandler : AuthorizationHandler<IsAdminRequirement>
    {
        private readonly CredentialManagerDbContext _db;

        public IsAdminAuthorizationHandler(CredentialManagerDbContext db)
        {
            _db = db;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminRequirement requirement)
        {
            var identity = context.User.Identity as ClaimsIdentity;
            var username = identity.Claims.FirstOrDefault(c => c.Type == Claims.PreferredUsername)?.Value;
            
            var adminUserExists = await _db.Users
                .AnyAsync(x => x.Role == Roles.Admin && x.Name == username);            

            if (adminUserExists)
            {
                context.Succeed(requirement);
            }
        }        
    }
}

