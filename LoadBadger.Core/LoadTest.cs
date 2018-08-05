using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.Core
{
    public abstract class LoadTest
    {
        private readonly IRequestReporter _requestReporter;
        protected readonly HttpClient HttpClient;

        protected LoadTest(IRequestReporter requestReporter)
        {
            _requestReporter = requestReporter;
            var timedHandler = new TimedHandler(_requestReporter, new SocketsHttpHandler());
            HttpClient = new HttpClient(timedHandler);
        }

        public async Task Run()
        {
            using (GetReporterTimer())
            {
                await GetLoadTest();
            }
        }

        protected abstract Task GetLoadTest();

        private Timer GetReporterTimer()
        {
            return new Timer(state =>
            {
                System.Console.Clear();
                _requestReporter.GetStatistics();
            }, 
            null, 0, 2000);
        }
    }
}