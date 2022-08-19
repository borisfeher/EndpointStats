using EnsureThat;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EndpointStats
{
    /// <summary>
    /// A middleware for logging the count of endpoint calls. Can affect performance.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly Dictionary<string, int> _requestCollection;
        private readonly Stopwatch _stopwatch;
        private int _countAllEndPointCalls;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestLoggingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The <see cref="RequestDelegate"/> representing the next middleware in the pipeline.</param>
        /// <param name="logger">Logger for <see cref="RequestLoggingMiddleware"/>.</param>
        /// <param name="config">The <see cref="IConfiguration"/> to add to.</param>
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IConfiguration config)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _requestCollection = new Dictionary<string, int>();
            _countAllEndPointCalls = 0;
            _stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Executes the middleware.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            EnsureArg.IsNotNull(context, nameof(context));

            try
            {
                // RequestLoggingMiddleware is enabled only when the StatsCollectionEnable config value is set to true.
                if (Convert.ToBoolean(_config.GetSection("EndpointStatsConfig:StatsCollectionEnable")))
                {
                    using (new BenchmarkToken(_stopwatch))
                    {
                        // Call the next middleware
                        await _next(context);
                    }

                    if (_countAllEndPointCalls <= Convert.ToInt32(_config.GetSection("EndpointStatsConfig:StatsCollectionDepth")))
                    {
                        StartCollection(context);
                    }
                    else
                    {
                        EndCollection();
                    }
                }
                else
                {
                    _logger.LogInformation("RequestLoggingMiddleware is disabled, config parameter 'StatsCollectionEnable' is set to false.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public void StartCollection(HttpContext context)
        {
            int count;
            if (_requestCollection.TryGetValue(context.Request.Path.Value, out count))
            {
                _requestCollection[context.Request.Path.Value] = count + 1;
            }
            else
            {
                _requestCollection.Add(context.Request.Path.Value, 1);
            }

            _countAllEndPointCalls++;
        }

        public void EndCollection()
        {
            _stopwatch.Stop();

            var mostCalledEndPoint = _requestCollection.Aggregate((l, r) => l.Value > r.Value ? l : r);
            _logger.LogInformation("Endpoint stats: {ENDP} called {M} times in {0:000}", mostCalledEndPoint.Key, mostCalledEndPoint.Value, _stopwatch.Elapsed.TotalSeconds);
        }

    }
}

