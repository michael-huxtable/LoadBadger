using System.Net.Http;
using System.Threading.Tasks;

namespace LoadBadger.ConsolerRunner
{
    public class HttpExecutor : IExecutor
    {
        public TimedHandler TimedHandler { get; }

        public HttpExecutor(TimedHandler timedHandler)
        {
            TimedHandler = timedHandler;
        }

        public Task ExecuteAsync()
        {
            var httpClient = new HttpClient(TimedHandler);
            return httpClient.GetAsync("http://localhost");
        }
    }
}