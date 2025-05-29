using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class AuthorizeIfNotTestAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var env = context.HttpContext.RequestServices.GetService<IHostEnvironment>();
        if (env != null && env.IsEnvironment("Test"))
        {
            // Skip authorization in the Test environment
            return;
        }

        var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();
        var policyProvider = context.HttpContext.RequestServices.GetService<IAuthorizationPolicyProvider>();

        // Get the default policy
        var policy = policyProvider.GetDefaultPolicyAsync().Result;

        // Check if the current user meets the policy requirements
        var result = authorizationService.AuthorizeAsync(context.HttpContext.User, null, policy).Result;

        if (!result.Succeeded)
        {
            context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
        }
    }
}
