using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.Core
{
    public class HttpExecutor : IExecutor
    {
        private readonly IRequestReporter _requestReporter;
        private readonly HttpClient _httpClient;
        
        public HttpExecutor(HttpClient httpClient, IRequestReporter requestReporter)
        {
            _httpClient = httpClient;
            _requestReporter = requestReporter;
        }
        
        public Task ExecuteAsync(CancellationTokenSource ctx)
        {
            var task = _httpClient.GetAsync("http://localhost");
            _requestReporter.InProgressRequests.Add(task);
            return task;
        }
    }
}