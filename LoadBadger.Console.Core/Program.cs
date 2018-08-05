using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LoadBadger.Core;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace LoadBadger.Console.Core
{
    public class SimpleLoadTest : LoadTest
    {
        public SimpleLoadTest() : base(new RequestReporter())
        {}

        protected override Task GetLoadTest()
        {
            Func<Task> test = async () =>
            {
                var data = await HttpClient.GetAsync("http://localhost");
                data.EnsureSuccessStatusCode();
                await data.Content.ReadAsStringAsync();
            };

            test.LinearRamp(start: 150, end: 200, duration: TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            SimpleLoadTest test = new SimpleLoadTest();
            test.Run().GetAwaiter().GetResult();
            return;

            var reporter = new RequestReporter();
            var timedHandler = new TimedHandler(reporter, new SocketsHttpHandler());
            var httpClient = new HttpClient(timedHandler);

            Func<Task> httpExecutor = () => httpClient.GetAsync("http://localhost", CancellationToken.None);

            Task.Run(async () =>
            {
                var cancellationToken = new CancellationTokenSource();

                Func<Task> task = async () =>
                {
                    var data = await httpClient.GetAsync("http://localhost", cancellationToken.Token);
                    data.EnsureSuccessStatusCode();
                    await data.Content.ReadAsStringAsync();
                };

                task.LinearRamp(start: 3000, end: 5000, duration: TimeSpan.FromMinutes(1));
                
                new LinearRampedHandlerLoop(start: 3000, end: 5000, duration: TimeSpan.FromMinutes(1), executor: task)
                    .Execute(cancellationToken);

                await Task.Delay(TimeSpan.FromSeconds(20), cancellationToken.Token);
                new PerSecondHandlerLoop(100, TimeSpan.FromMinutes(1), executor: task).Execute(cancellationToken);
            });

            Task.Run(() =>
            {
                while (true)
                {
                    reporter.GetStatistics();

                    Thread.Sleep(2000);
                    System.Console.Clear();
                }
            });

            System.Console.ReadKey();
            Task.WaitAll(reporter.InProgressRequests.ToArray());

            foreach (var request in reporter.CompletedRequests)
            {
                System.Console.WriteLine($"Request: {request.Start.Ticks} - {request.End.Ticks} in {request.Total.TotalMilliseconds}ms");
            }

            System.Console.WriteLine("Total:" + reporter.CompletedRequests.Count);
        }
    }
}