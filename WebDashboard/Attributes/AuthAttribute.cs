using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace WebDashboard.Attributes
{
    public class AuthMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.ToString() == "/index.html" && !ShouldSkipController(context))
            {
                if (context.Session.GetInt32("IsAuthorized") != 1)
                {
                    context.Response.Redirect("/Authorize");
                    return;
                }
            }
            await next.Invoke(context);
        }

        private bool ShouldSkipController(HttpContext context)
        {
            var controllerTypeInfo = context
                .GetEndpoint()
                ?.Metadata
                ?.GetMetadata<ControllerActionDescriptor>()
                ?.ControllerTypeInfo;

            var skipAttribute = controllerTypeInfo?.GetCustomAttribute<IgnoreMiddlewareAttribute>();

            return skipAttribute != null;
        }

    }

    public class AuthAttribute: AuthorizeFilter
    {
        public override Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {

            return base.OnAuthorizationAsync(context);
        }
    }
}