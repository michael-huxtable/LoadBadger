using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace LoadBadger.ConsolerRunner
{
    public class LinearRampedHandlerLoop
    {
        private readonly IExecutor _executor;

        public LinearRampedHandlerLoop(IExecutor executor)
        {
            _executor = executor;
        }

        public List<Task> Execute(int start, int end, TimeSpan duration, CancellationTokenSource ctx)
        {
            TimeSpan rampInterval = TimeSpan.FromSeconds(1);

            double rampCount = duration.TotalSeconds / rampInterval.TotalSeconds;

            int step = end - start;
            double stepCount = step / rampCount;
            double requestsPerSecond = start;

            var tasks = new List<Task>();

            for (TimeSpan i = TimeSpan.Zero; i < duration; i += rampInterval)
            {
                Log.Information("Requests Per Second: {perSecond}, Time: {i}", requestsPerSecond, i);

                if (requestsPerSecond > 0)
                {   
                    var loop = new PerSecondHandlerLoop(requestsPerSecond, rampInterval, _executor);
                    tasks.AddRange(loop.Exeucte(new CancellationTokenSource()));
                }

                requestsPerSecond += stepCount;
            }

            return tasks;
        }
    }
}