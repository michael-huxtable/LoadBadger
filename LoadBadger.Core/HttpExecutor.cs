using System.Net.Http;
using System.Threading.Tasks;

namespace LoadBadger.Core
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