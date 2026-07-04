using RadarSantista.src.Repositories;
using RadarSantista.src.Services;

namespace RadarSantista.src
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var httpClient = SetupHttpClient();
            INavioRepository navioRepository = new DatabaseRepository();
            var scraperService = new ScraperService(httpClient);
            var consoleOutput = new ConsoleOutput();
            var monitoringService = new MonitoringService(navioRepository, scraperService, consoleOutput);

            await monitoringService.RunAsync();
        }

        private static HttpClient SetupHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            return httpClient;
        }
    }
}