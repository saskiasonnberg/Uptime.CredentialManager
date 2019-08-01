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
        private readonly UptimeCredentialManagerWebContext db;

        public IsAdminAuthorizationHandler(UptimeCredentialManagerWebContext db)
        {
            this.db = db;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminRequirement requirement)
        {
            var identity = context.User.Identity as ClaimsIdentity;
            string preferred_username = identity.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            
            var userCompare = await db.User
                                   .Include(x => x.UserCredentials)
                                   .ThenInclude(x => x.Credential)
                                   .Where(x => x.Role == Roles.Admin)
                                   .FirstOrDefaultAsync(m => m.Name == preferred_username);            

            if (userCompare != null)
            {                                          
                context.Succeed(requirement);
            }
        }        
    }
}

