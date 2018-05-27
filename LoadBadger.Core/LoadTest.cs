using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.Core
{
    public class LoadTest
    {
        private readonly IEnumerable<IExecutor> _steps;

        public LoadTest(IEnumerable<IExecutor> steps)
        {
            _steps = steps;
        }

        public async Task Run()
        {
            var ctx = new CancellationTokenSource();

            foreach (IExecutor executor in _steps)
            {
                await executor.ExecuteAsync(ctx);
            }
        }
    }
}