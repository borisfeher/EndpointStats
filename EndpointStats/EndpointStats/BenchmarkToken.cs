using System;
using System.Diagnostics;

namespace EndpointStats
{
    /// <summary>
    /// This class ensures that a stopwatch is stopped when an exception occurs without 
    /// needing multiple <see cref="Stopwatch.Stop"/> calls.
    /// </summary>
    public class BenchmarkToken : IDisposable
    {
        private readonly Stopwatch _stopwatch;

        public BenchmarkToken(Stopwatch stopwatch)
        {
            _stopwatch = stopwatch;
            _stopwatch.Start();
        }

        public void Dispose() => _stopwatch.Stop();
    }
}
