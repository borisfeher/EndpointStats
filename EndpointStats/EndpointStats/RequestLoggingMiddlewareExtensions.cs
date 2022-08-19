using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace EndpointStats
{
    /// <summary>
    /// Provides extension methods for registering the request logging middleware.
    /// </summary>
    public static class RequestLoggingMiddlewareExtensions
    {
        /// <summary>
        /// Adds the request logging middleware to the application pipeline.
        /// </summary>
        /// <param name="builder">An application builder.</param>
        public static IApplicationBuilder UseRequestLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }

        /// <summary>
        /// Adds the request logging middleware using an <see cref="IStartupFilter"/> so that it can run earlier in the pipeline.
        /// </summary>
        /// <param name="builder">A webhost builder.</param>
        public static IWebHostBuilder UseRequestLoggingMiddleware(this IWebHostBuilder builder)
        {
            return builder.ConfigureServices(services =>
                services.AddSingleton<IStartupFilter>(new RequestLoggingStartupFilter()));
        }
    }
}
