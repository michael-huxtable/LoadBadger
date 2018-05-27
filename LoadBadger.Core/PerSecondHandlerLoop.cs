using System;
using System.Threading;

namespace LoadBadger.Core
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

        protected override void OnUpdate(CancellationTokenSource cancellationToken)
        { 
            if (Stopwatch.Elapsed >= _duration && _totalExecutions >= _expectedExecutions)
            {
                cancellationToken.Cancel();
            }

            double current = _excutionsPerFrame + _remainder;
            double truncated = Math.Truncate(current);
            _remainder += _excutionsPerFrame - truncated;
        
            for (int i = 0; i < truncated; ++i)
            {
                _totalExecutions++;
                _executor.ExecuteAsync(cancellationToken);
            }
        }
    }
}