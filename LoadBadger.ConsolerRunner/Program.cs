using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.ConsolerRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Is High Resolution: {Stopwatch.IsHighResolution}");
            var timedHandler = new TimedHandler();
            HttpClient httpClient = new HttpClient(timedHandler);

            Task.Run(() =>
            {
                var looper = new FrequencyLoop(fps: 240);
                looper.Update += (sender, eventArgs) =>
                {
                    httpClient.GetAsync("http://localhost");
                };
                looper.Exeucte();
            });

            Task.Run(() =>
            {
                while (true)
                {
                    if (!timedHandler.Requests.Any())
                    {
                        continue;
                    }

                    Console.WriteLine("Total Requests Sent: " + timedHandler.Requests.Count);
                    Console.WriteLine("Avg Response Time: " + timedHandler.Requests.Average(r => r.Total.TotalMilliseconds));
                    Console.WriteLine("Total RPS: " + timedHandler.Requests
                        .GroupBy(r => new DateTime(r.Start.Year, r.Start.Month, r.Start.Day, r.Start.Hour, r.Start.Minute, r.Start.Second))
                        .Average(r => r.Count()));
                    Thread.Sleep(1000);
                }
            });

            Console.WriteLine("Started!");
            Console.ReadKey();

            foreach (var request in timedHandler.Requests)
            {
                Console.WriteLine($"Request: {request.Start.Ticks} - {request.End.Ticks} in {request.Total.TotalMilliseconds}ms");
            }

            Console.WriteLine("Total:" + timedHandler.Requests.Count);
        }
    }
}
