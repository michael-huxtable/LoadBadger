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
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            var timedHandler = new TimedHandler();
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
                    if (!timedHandler.Requests.Any())
                    {
                        continue;
                    }

                    var snapshot = timedHandler.Requests.Select(r => new RequestTime(r)).ToList();

                    Log.Information("Total Requests Sent: {total}", snapshot.Count);
                    Log.Information("Avg Response Time: {0:N}ms", snapshot.Average(r => r.Total.TotalMilliseconds));

                    var averageRps = snapshot
                        .GroupBy(r => new DateTime(r.Start.Year, r.Start.Month, r.Start.Day, r.Start.Hour, r.Start.Minute, r.Start.Second))
                        .Average(r => r.Count());

                    Log.Information("Average RPS: {0:N}", averageRps);
                    Log.Information("Requests < 800ms: {total}", snapshot.Count(r => r.Total.TotalMilliseconds < 800));
                    Log.Warning("Requests > 800ms <= 1000ms: {total}", snapshot.Count(r => r.Total.TotalMilliseconds > 800 && r.Total.TotalMilliseconds <= 1000));
                    Log.Error("Requests > 1000ms: {total}", snapshot.Count(r => r.Total.TotalMilliseconds > 1000));

                    Thread.Sleep(1000);
                    System.Console.Clear();
                }
            });

            System.Console.ReadKey();
            Task.WaitAll(tasks.ToArray());

            foreach (var request in timedHandler.Requests)
            {
                System.Console.WriteLine($"Request: {request.Start.Ticks} - {request.End.Ticks} in {request.Total.TotalMilliseconds}ms");
            }

            System.Console.WriteLine("Total:" + timedHandler.Requests.Count);
        }
    }
}