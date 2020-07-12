using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace LoadBadger.Core
{
    public class RequestReporter : IRequestReporter
    {
        public ConcurrentBag<RequestTime> CompletedRequests { get; } = new ConcurrentBag<RequestTime>();

        public ConcurrentBag<Task> InProgressRequests { get; } = new ConcurrentBag<Task>();

        public void GetStatistics()
        {
            if (!CompletedRequests.Any())
            {
                return;
            }

            var snapshot = CompletedRequests.ToList();

            Log.Information("Total CompletedRequests Sent: {total}", snapshot.Count);
            Log.Information("Avg Response Time: {0:N}ms", snapshot.Average(r => r.Total.TotalMilliseconds));

            var toTheSecond = snapshot
                .GroupBy(r => r.Start.Truncate(TimeSpan.FromSeconds(1)))
                .ToList();

            var averageRps = toTheSecond.Average(r => r.Count());
            Log.Information("Average RPS: {0:N}", averageRps);
            Log.Information("Requests < 800ms: {total}", snapshot.Count(r => r.Total.TotalMilliseconds < 800));
            Log.Warning("Requests > 800ms <= 1000ms: {total}", snapshot.Count(r => r.Total.TotalMilliseconds > 800 && r.Total.TotalMilliseconds <= 1000));
            Log.Error("Requests > 1000ms: {total}", snapshot.Count(r => r.Total.TotalMilliseconds > 1000));
        }
    }
}