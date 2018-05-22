using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.ConsolerRunner
{
    public class PerSecondHandlerLoop : FrequencyLoop
    {
        private readonly TimeSpan _duration;
        private readonly IExecutor _executor;

        private readonly double _excutionsPerFrame;
        private readonly double _expectedExecutions;

        public PerSecondHandlerLoop(double perSecond, TimeSpan duration, IExecutor executor) : base(240)
        {
            _duration = duration;
            _executor = executor;
            _excutionsPerFrame = perSecond / 240;
            _expectedExecutions = perSecond * duration.TotalSeconds;
        }

        private double _totalExecutions;
        private double _remainder;

        protected override IEnumerable<Task> OnUpdate(CancellationTokenSource cancellationToken)
        {
            if (Stopwatch.Elapsed >= _duration && _totalExecutions >= _expectedExecutions)
            {
                cancellationToken.Cancel();
                return new List<Task>();
            }

            double current = _excutionsPerFrame + _remainder;
            double truncated = Math.Truncate(current);
            _remainder += _excutionsPerFrame - truncated;

            var tasks = new List<Task>();

            for (int i = 0; i < truncated; ++i)
            {
                _totalExecutions++;
                tasks.Add(_executor.ExecuteAsync());
            }

            return tasks;
        }
    }
}