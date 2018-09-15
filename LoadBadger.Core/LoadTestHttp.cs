using System.Net.Http;

namespace LoadBadger.Core
{
    public static class LoadTestHttp
    {
        public static IRequestReporter RequestReporter { get; } = new RequestReporter();

        public static readonly TimedHandler TimedHandler = new TimedHandler(RequestReporter, new SocketsHttpHandler());

        public static HttpClient BuildClient() => new HttpClient(TimedHandler);
    }
}