using System;
using System.Collections.Concurrent;
using System.Linq;
using Serilog;

namespace LoadBadger.Core
{
    public class RequestTimeReporter
    {
        public ConcurrentBag<RequestTime> Requests { get; } = new ConcurrentBag<RequestTime>();

        public void GetStatistics()
        {
            if (!Requests.Any())
            {
                return;
            }

            var snapshot = Requests.Select(r => new RequestTime(r)).ToList();

            Log.Information("Total Requests Sent: {total}", snapshot.Count);
            Log.Information("Avg Response Time: {0:N}ms", snapshot.Average(r => r.Total.TotalMilliseconds));

            var toTheSecond = snapshot
                .GroupBy(r => r.Start.Truncate(TimeSpan.FromSeconds(1)))
                .ToList();

            var averageRps = toTheSecond.Average(r => r.Count());
            Log.Information("Average RPS: {0:N}", averageRps);

            //var lastRps = toTheSecond
            //    .OrderByDescending(r => r.Key).First()
            //    .Count();

            //Log.Information("Last RPS: {0:N}", lastRps);
            Log.Information("Requests < 800ms: {total}", snapshot.Count(r => r.Total.TotalMilliseconds < 800));
            Log.Warning("Requests > 800ms <= 1000ms: {total}", snapshot.Count(r => r.Total.TotalMilliseconds > 800 && r.Total.TotalMilliseconds <= 1000));
            Log.Error("Requests > 1000ms: {total}", snapshot.Count(r => r.Total.TotalMilliseconds > 1000));
        }
    }
}