using Microsoft.AspNetCore.Authorization;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace ECommerce.Security
{
    public class UserCanOnlyAccessAndEditHisDataRequirement : IAuthorizationRequirement
    {


    }

    public class UserCanAccessOnlyHisDataHandler : AuthorizationHandler<UserCanOnlyAccessAndEditHisDataRequirement, Document>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            UserCanOnlyAccessAndEditHisDataRequirement requirement, Document resource)
        {
            if (context.User.HasClaim(c => c.Type == ClaimTypes.Name))
            {
                return Task.CompletedTask;
            }
            var res = resource.Name;
            var hi = context.Resource;
            var user = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
