using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.ConsolerRunner
{
    public class PerSecondHandlerLoop : FrequencyLoop
    {
        private readonly HttpExecutor _executor;

        private readonly decimal _excutionsPerFrame;
        private readonly decimal _expectedExecutions;

        public PerSecondHandlerLoop(decimal perSecond, TimeSpan duration, HttpExecutor executor) : base(480)
        {
            _executor = executor;
            _excutionsPerFrame = perSecond / 480;
            _expectedExecutions = perSecond * (decimal) duration.TotalSeconds;
        }

        private decimal _totalExecutions;
        private decimal _remainder;

        protected override IEnumerable<Task> OnUpdate()
        {
            if (_totalExecutions >= _expectedExecutions)
            {
                yield break;
            }

            decimal current = _excutionsPerFrame + _remainder;
            decimal truncated = Math.Truncate(current);
            _remainder += _excutionsPerFrame - truncated;

            for (int i = 0; i < truncated; ++i)
            {
                _totalExecutions++;
                yield return _executor.ExecuteAsync();
            }
        }
    }
}