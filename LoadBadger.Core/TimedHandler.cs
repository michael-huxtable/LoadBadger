using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.Core
{
    public class TimedHandler : DelegatingHandler
    {
        private readonly IRequestReporter _requestReporter;

        public TimedHandler(IRequestReporter requestReporter, HttpMessageHandler innerHandler) : base(innerHandler)
        {
            _requestReporter = requestReporter;
        }
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var startDate = HighResolutionDateTime.UtcNow;
            
            var task = base.SendAsync(request, cancellationToken);
            _requestReporter.InProgressRequests.Add(task);
            var response = await task;
            
            var endDate = HighResolutionDateTime.UtcNow;

            var requestTime = new RequestTime(startDate, endDate);
            _requestReporter.CompletedRequests.Add(requestTime);
            
            return response;
        }
    }
}