using RadarSantista.ConsoleApp.Models;
using RadarSantista.ConsoleApp.Services;
using RadarSantista.ConsoleApp.Repositories;

namespace RadarSantista.ConsoleApp
{
    public class Program
    {
        private static readonly HttpClient client = SetupHttpClient();
        private static List<Navio> cacheConsolidado = new List<Navio>();

        public static async Task Main(string[] args)
        {
            var db = new DatabaseRepository();
            var scraper = new ScraperService(client);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"[RadarSantista] Executado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine(new string('=', 102));

                Console.WriteLine("Coletando dados do Porto:");
                var dadosAtracados = await scraper.ObterAtracados();
                var dadosProgramados = await scraper.ObterProgramados();

                if (dadosAtracados.Count > 0 || dadosProgramados.Count > 0)
                {
                    cacheConsolidado = DataEngine.Consolidar(dadosAtracados, dadosProgramados);
                    
                    db.SalvarAtracados(dadosAtracados);
                    db.SalvarProgramados(dadosProgramados);
                }
                else if (cacheConsolidado.Count > 0)
                {
                    Console.WriteLine("Falha ao atualizar.");
                }

                ConsoleVisualService.ExibirPainel(cacheConsolidado);

                Console.WriteLine("\nAguarde 1 min para o próximo monitoramento:");
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
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