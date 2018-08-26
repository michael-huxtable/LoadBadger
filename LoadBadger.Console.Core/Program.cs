using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LoadBadger.Core;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace LoadBadger.Console.Core
{
    public class SimpleLoadTest
    {
        public Func<Task> GetTaskFunc => GetTest;

        public async Task GetTest()
        {
            HttpClient httpClient = LoadTestHttp.BuildClient();

            var data = await httpClient.GetAsync("http://localhost");
            data.EnsureSuccessStatusCode();
            await data.Content.ReadAsStringAsync();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            using (var timer = GetReporterTimer())
            {
                var two = new SimpleLoadTest();
                two.GetTaskFunc.LinearRamp(500, 1000, TimeSpan.FromMinutes(1));

                System.Console.ReadKey();

                var requestReporter = LoadTestHttp.RequestReporter;
                Task.WaitAll(requestReporter.InProgressRequests.ToArray());

                foreach (var request in requestReporter.CompletedRequests)
                {
                    System.Console.WriteLine(
                        $"Request: {request.Start.Ticks} - {request.End.Ticks} in {request.Total.TotalMilliseconds}ms");
                }

                System.Console.WriteLine("Total:" + requestReporter.CompletedRequests.Count);
            }
        }

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