using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.ConsolerRunner
{
    public class LinearRampedHandlerLoop
    {
        private readonly HttpExecutor _httpExecutor;

        public LinearRampedHandlerLoop(HttpExecutor httpExecutor)
        {
            _httpExecutor = httpExecutor;
        }

        public List<Task> Execute(CancellationTokenSource ctx)
        {
            int start = 10;
            int end = 20;
            TimeSpan duration = TimeSpan.FromSeconds(30);
            TimeSpan rampInterval = TimeSpan.FromSeconds(1);

            double rampCount = duration.TotalSeconds / rampInterval.TotalSeconds;

            int step = end - start;
            double count = step / rampCount;
            double perSecond = start;

            var tasks = new List<Task>();

            for (TimeSpan i = TimeSpan.Zero; i < duration; i += rampInterval)
            {
                Console.WriteLine($"Count: {perSecond}, Time: {i}");
                if (perSecond > 0)
                {
                    var loop = new PerSecondHandlerLoop((decimal) perSecond, rampInterval, _httpExecutor);
                    tasks.AddRange(loop.Exeucte(new CancellationTokenSource()));
                }

                perSecond += count;
            }

            return tasks;
        }
    }
}