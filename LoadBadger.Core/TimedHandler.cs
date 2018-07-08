using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.Core
{
    public class TimedHandler : DelegatingHandler
    {
        private readonly ConcurrentBag<RequestTime> _requests;

        public TimedHandler(ConcurrentBag<RequestTime> requests) : base(new HttpClientHandler())
        {
            _requests = requests;
        }
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var startDate = HighResolutionDateTime.UtcNow;
            var response = await base.SendAsync(request, cancellationToken);
            var endDate = HighResolutionDateTime.UtcNow;
            
            _requests.Add(new RequestTime(startDate, endDate));
            return response;
        }
    }
}