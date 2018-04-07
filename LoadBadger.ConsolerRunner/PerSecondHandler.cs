using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoadBadger.ConsolerRunner
{
    public class PerSecondHandler
    {
        private readonly HttpExecutor _executor;
        private readonly decimal _perFrame;

        public PerSecondHandler(decimal fps, decimal perSecond, HttpExecutor executor)
        {
            _executor = executor;
            _perFrame = perSecond / fps;
        }

        private decimal _remainder;
        
        public IEnumerable<Task> ExecuteAsync() 
        {
            decimal current = _perFrame + _remainder;
            decimal truncated = Math.Truncate(current);
            _remainder += _perFrame - truncated;
            
            for (int i = 0; i < truncated; ++i)
            {
                yield return _executor.ExecuteAsync();
            }
        }
    }
}