using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.Core
{
    public class LoadTestUser : IExecutor
    {
        private HttpClient _httpClient = new HttpClient();


        public Task ExecuteAsync(CancellationTokenSource cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class LoadTest
    {
        private readonly IEnumerable<IExecutor> _steps;

        public LoadTest(IEnumerable<IExecutor> steps)
        {
            _steps = steps;
        }

        public Task ExecuteAsync()
        {
            var ctx = new CancellationTokenSource();
            
            return Task.Run(async () =>
            {
                foreach (IExecutor executor in _steps)
                {
                    await executor.ExecuteAsync(ctx);
                }
            }, 
            ctx.Token);
        }
    }
}