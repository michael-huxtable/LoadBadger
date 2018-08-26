using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.Core
{
    public static class LoadTestHttp
    {
        public static IRequestReporter RequestReporter { get; } = new RequestReporter();
        public static TimedHandler TimedHandler { get; } = new TimedHandler(RequestReporter, new SocketsHttpHandler());

        public static HttpClient BuildClient() => new HttpClient(TimedHandler);
    }
}