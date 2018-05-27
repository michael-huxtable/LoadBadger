using System;
using System.Collections.Generic;
using System.Linq;
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
            var httpExecutor = new HttpExecutor(timedHandler, reporter);

            List<Task> tasks = new List<Task>();

            Task.Run(() =>
            {
                var ctx = new CancellationTokenSource();

                var ramped = new LinearRampedHandlerLoop(start: 50, end: 100, duration: TimeSpan.FromMinutes(1), executor: httpExecutor);
                return ramped.ExecuteAsync(ctx);

                //var perSecond = new PerSecondHandlerLoop(10, TimeSpan.FromSeconds(30), httpExecutor);
                //tasks = perSecond.Exeucte(ctx.Token);
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