using System;
using System.Net.Http;
using System.Threading.Tasks;
using LoadBadger.Core;

namespace LoadBadger.Console.Core
{
    public class Test : LoadTest
    {
        private readonly HttpClient _httpClient = LoadTestHttp.BuildClient();

        protected override void Execute()
        {
            Func<Task> scenario = Scenario;
            scenario.LinearRamp(2000, 3000, TimeSpan.FromMinutes(1));
        }
   
        public async Task Scenario()
        {
            var data = await _httpClient.GetAsync("http://localhost");
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