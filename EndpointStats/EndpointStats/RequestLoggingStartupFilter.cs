using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace EndpointStats
{
    /// <summary>
    /// An <see cref="IStartupFilter"/> that adds request logging middleware.
    /// </summary>
    class RequestLoggingStartupFilter : IStartupFilter
    {
        public RequestLoggingStartupFilter() { }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) =>
            app => next(app.UseRequestLoggingMiddleware());
    }
}
