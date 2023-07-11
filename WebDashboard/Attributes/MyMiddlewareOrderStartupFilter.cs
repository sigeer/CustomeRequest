using Microsoft.AspNetCore.Builder;

namespace WebDashboard.Attributes
{
    public class MyMiddlewareOrderStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseSession();
                // 在这里调整你的中间件的顺序
                builder.UseMiddleware<AuthMiddleware>();

                // 调用额外托管程序集中的中间件
                next(builder);
            };
        }
    }

}
