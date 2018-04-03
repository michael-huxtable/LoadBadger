using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.ConsolerRunner
{
    public class TimedHandler : DelegatingHandler
    {
        public TimedHandler() : base(new HttpClientHandler())
        {}

        public ConcurrentBag<RequestTime> Requests { get; } = new ConcurrentBag<RequestTime>();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var startDate = HighResolutionDateTime.UtcNow;
            var response = await base.SendAsync(request, cancellationToken);
            var endDate = HighResolutionDateTime.UtcNow;
            
            Requests.Add(new RequestTime(startDate, endDate));
            return response;
        }
    }
}