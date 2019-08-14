using Microsoft.AspNetCore.Authorization;

namespace Uptime.CredentialManager.Web.Authorization
{
    public class IsAdminPolicy
    {
        public const string Name = "IsAdmin";
        public static AuthorizationPolicy Policy => new AuthorizationPolicyBuilder()
            .AddRequirements(new IsAdminRequirement())
            .Build();
    }
}
