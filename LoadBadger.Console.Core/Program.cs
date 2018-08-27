using System;
using System.Net.Http;
using System.Threading.Tasks;
using LoadBadger.Core;

namespace LoadBadger.Console.Core
{
    public class Test : LoadTest
    {
        protected override void Execute()
        {
            Func<Task> scenario = Scenario;
            scenario.LinearRamp(500, 1000, TimeSpan.FromMinutes(1));
        }

        public async Task Scenario()
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
            var test = new Test();
            test.Run();
        }
    }
}