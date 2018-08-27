using System;
using System.Threading;
using System.Threading.Tasks;
using LoadBadger.Core;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace LoadBadger.Console.Core
{
    public abstract class LoadTest
    {
        public void Run()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            using (GetReporterTimer())
            {
                var task = Task.Run((Action)Execute);
                System.Console.ReadKey();

                var requestReporter = LoadTestHttp.RequestReporter;

                foreach (var request in requestReporter.CompletedRequests)
                {
                    Log.Information("Request: {start} - {end} in {total}ms",
                        request.Start.Ticks, request.End.Ticks, request.Total.TotalMilliseconds);
                }

                Log.Information("Total: {count}", requestReporter.CompletedRequests.Count);
            }
        }

        protected abstract void Execute();

        private static Timer GetReporterTimer()
        {
            return new Timer(state =>
            {
                System.Console.Clear();
                LoadTestHttp.RequestReporter.GetStatistics();
            },
            null, 0, 2000);
        }
    }
}