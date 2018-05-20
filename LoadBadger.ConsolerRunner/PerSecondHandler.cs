using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.ConsolerRunner
{
    public class PerSecondHandlerLoop : FrequencyLoop
    {
        private readonly TimeSpan _duration;
        private readonly HttpExecutor _executor;

        private readonly decimal _excutionsPerFrame;
        private readonly decimal _expectedExecutions;

        public PerSecondHandlerLoop(decimal perSecond, TimeSpan duration, HttpExecutor executor) : base(30)
        {
            _duration = duration;
            _executor = executor;
            _excutionsPerFrame = perSecond / 30;
            _expectedExecutions = perSecond * (decimal) duration.TotalSeconds;
        }

        private decimal _totalExecutions;
        private decimal _remainder;

        protected override IEnumerable<Task> OnUpdate(CancellationTokenSource cancellationToken)
        {
            if (Stopwatch.Elapsed >= _duration && _totalExecutions >= _expectedExecutions)
            {
                cancellationToken.Cancel();
                return new List<Task>();
            }

            decimal current = _excutionsPerFrame + _remainder;
            decimal truncated = Math.Truncate(current);
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