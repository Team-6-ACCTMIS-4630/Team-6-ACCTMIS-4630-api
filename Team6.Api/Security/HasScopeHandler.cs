using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Team6.Api.Security
{
    // Checks whether the authenticated user has the required Auth0 permission.
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirements>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            HasScopeRequirements requirement)
        {
            // Verify that the token contains a scope claim from the trusted issuer.
            if (!context.User.HasClaim(c =>
                c.Type == "scope" && c.Issuer == requirement.Issuer))
            {
                return Task.CompletedTask;
            }

            // A scope claim can contain multiple permissions separated by spaces.
            var scopes = context.User
                .FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer)?
                .Value.Split(' ');

            // Grant authorization only when the required permission exists.
            if (scopes?.Any(s => s == requirement.Scope) == true)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}