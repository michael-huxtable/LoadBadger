using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.Core
{
    public class HttpExecutor : IExecutor
    {
        private readonly IRequestReporter _requestReporter;
        public TimedHandler TimedHandler { get; }

        public HttpExecutor(TimedHandler timedHandler, IRequestReporter requestReporter)
        {
            TimedHandler = timedHandler;
            _requestReporter = requestReporter;
        }

        public Task ExecuteAsync(CancellationTokenSource ctx)
        {
            var httpClient = new HttpClient(TimedHandler);

            var task = httpClient.GetAsync("http://localhost");
            _requestReporter.InProgressRequests.Add(task);
            return task;
        }
    }
}