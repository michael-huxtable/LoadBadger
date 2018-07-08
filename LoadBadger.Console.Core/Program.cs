using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LoadBadger.Core;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace LoadBadger.Console.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            var reporter = new RequestReporter();

            var timedHandler = new TimedHandler(reporter.CompletedRequests);
            var httpExecutor = new HttpExecutor(new HttpClient(timedHandler), reporter);

            Task.Run(async () =>
            {
                var cancellationToken = new CancellationTokenSource();
                new LinearRampedHandlerLoop(start: 50, end: 100, duration: TimeSpan.FromMinutes(1), executor: httpExecutor).Execute(cancellationToken);
                await Task.Delay(TimeSpan.FromSeconds(20), cancellationToken.Token);
                new PerSecondHandlerLoop(100, TimeSpan.FromMinutes(1), executor: httpExecutor).Execute(cancellationToken);
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