using System;
using System.Collections.Generic;
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
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            var reporter = new RequestTimeReporter();

            var timedHandler = new TimedHandler(reporter.Requests);
            var httpExecutor = new HttpExecutor(timedHandler);
            List<Task> tasks = new List<Task>();

            Task.Run(() =>
            {
                var ctx = new CancellationTokenSource();

                var ramped = new LinearRampedHandlerLoop(httpExecutor);
                tasks = ramped.Execute(start: 1000, end: 5000, duration: TimeSpan.FromMinutes(1), ctx: ctx);

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
            Task.WaitAll(tasks.ToArray());

            foreach (var request in reporter.Requests)
            {
                System.Console.WriteLine($"Request: {request.Start.Ticks} - {request.End.Ticks} in {request.Total.TotalMilliseconds}ms");
            }

            System.Console.WriteLine("Total:" + reporter.Requests.Count);
        }
    }
}