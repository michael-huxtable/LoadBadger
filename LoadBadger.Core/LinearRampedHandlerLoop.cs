using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace LoadBadger.Core
{
    public static class LinearRampedHandlerLoopExtensions
    {
        public static void LinearRamp(this Func<Task> task, int start, int end, TimeSpan duration)
        {
            var loop = new LinearRampedHandlerLoop(start, end, duration, task);
            loop.Execute(new CancellationTokenSource());
        }
    }

    public class LinearRampedHandlerLoop
    {
        private readonly int _start;
        private readonly int _end;
        private readonly TimeSpan _duration;
        private readonly Func<Task> _executor;

        public LinearRampedHandlerLoop(int start, int end, TimeSpan duration, Func<Task> executor)
        {
            _start = start;
            _end = end;
            _duration = duration;
            _executor = executor;
        }

        public void Execute(CancellationTokenSource ctx)
        {
            TimeSpan rampInterval = TimeSpan.FromSeconds(1);

            double rampCount = _duration.TotalSeconds / rampInterval.TotalSeconds;

            int step = _end - _start;
            double stepCount = step / rampCount;
            double requestsPerSecond = _start;
            
            for (TimeSpan i = TimeSpan.Zero; i < _duration; i += rampInterval)
            {
                Log.Information("CompletedRequests Per Second: {perSecond}, Time: {i}", requestsPerSecond, i);

                if (requestsPerSecond > 0)
                {
                    var loop = new PerSecondHandlerLoop(requestsPerSecond, rampInterval, _executor);
                    loop.Execute(new CancellationTokenSource());
                }

                requestsPerSecond += stepCount;
            }
        }
    }
}