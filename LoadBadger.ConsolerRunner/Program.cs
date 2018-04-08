using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.ConsolerRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var timedHandler = new TimedHandler();
            var httpExecutor = new HttpExecutor(timedHandler);
            List<Task> tasks = new List<Task>();

            Task.Run(() =>
            {
                var ctx = new CancellationTokenSource();

                var perSecond = new PerSecondHandlerLoop(33, TimeSpan.FromSeconds(30), httpExecutor);
                tasks = perSecond.Exeucte(ctx.Token);
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

                    Console.WriteLine("Total Requests Sent: " + snapshot.Count);
                    Console.WriteLine("Avg Response Time: {0:N}ms", snapshot.Average(r => r.Total.TotalMilliseconds));
                    Console.WriteLine("Total RPS: {0:N}", snapshot
                        .GroupBy(r => new DateTime(r.Start.Year, r.Start.Month, r.Start.Day, r.Start.Hour,
                            r.Start.Minute, r.Start.Second))
                        .Average(r => r.Count()));

                    Console.WriteLine("Requests < 800ms: " + snapshot.Count(r => r.Total.TotalMilliseconds < 800));
                    Console.WriteLine("Requests > 800ms <= 1000ms: " + snapshot.Count(r =>
                                          r.Total.TotalMilliseconds > 800 && r.Total.TotalMilliseconds <= 1000));
                    Console.WriteLine("Requests > 1000ms: " + snapshot.Count(r => r.Total.TotalMilliseconds > 1000));

                    Thread.Sleep(1000);
                    Console.Clear();
                }
            });

            Console.ReadKey();
            Task.WaitAll(tasks.ToArray());

            foreach (var request in timedHandler.Requests)
            {
                Console.WriteLine(
                    $"Request: {request.Start.Ticks} - {request.End.Ticks} in {request.Total.TotalMilliseconds}ms");
            }

            Console.WriteLine("Total:" + timedHandler.Requests.Count);
        }
    }
}