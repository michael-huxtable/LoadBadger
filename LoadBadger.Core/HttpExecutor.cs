using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.Core
{
    public class HttpExecutor : IExecutor
    {
        private readonly HttpClient _httpClient;
        
        public HttpExecutor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public Task ExecuteAsync(CancellationTokenSource ctx)
        {
            var task = _httpClient.GetAsync("http://localhost");
            return task;
        }
    }
}