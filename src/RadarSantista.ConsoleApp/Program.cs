using RadarSantista.ConsoleApp.Models;
using RadarSantista.ConsoleApp.Services;

namespace RadarSantista.ConsoleApp
{
    public class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static List<Navio> cacheConsolidado = new List<Navio>();

        public static async Task Main(string[] args)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            var scraper = new ScraperService(client);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"[ROBÔ PORTO] Varredura executada em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine(new string('=', 115));

                Console.WriteLine("⏳ Coletando dados do Porto de Santos...");
                var dadosAtracados = await scraper.ObterAtracados();
                var dadosProgramados = await scraper.ObterProgramados();

                if (dadosAtracados.Count > 0 || dadosProgramados.Count > 0)
                {
                    cacheConsolidado = DataEngine.Consolidar(dadosAtracados, dadosProgramados);
                }
                else if (cacheConsolidado.Count > 0)
                {
                    Console.WriteLine("⚠️ Falha ao atualizar ⚠️ Exibindo dados do cache interno local:");
                }

                ExibirPainel(cacheConsolidado);

                Console.WriteLine("\n Aguardando 5 minutos para o próximo ciclo de monitoramento...");
                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }

        private static void ExibirPainel(List<Navio> navios)
{
    Console.Clear();
    Console.WriteLine($"⚙️ [RADAR SANTISTA] Atualizado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
    Console.WriteLine(new string('=', 115));

    // -----------------------------------------------------------------------------------
    // SEÇÃO 1: NAVIOS ATRACADOS AGORA (Quem já está operando ou acabou de encostar)
    // -----------------------------------------------------------------------------------
    Console.WriteLine("🚢 NAVIOS ATRACADOS NO PORTO (OPERANDO AGORA)");
    Console.WriteLine(new string('-', 115));
    Console.WriteLine($"{"SITUAÇÃO".PadRight(20)} | {"NAVIO".PadRight(22)} | {"TERMINAL / BERÇO".PadRight(22)} | {"MOVIMENTAÇÃO / CARGA"}");
    Console.WriteLine(new string('-', 115));

    var atracados = navios.Where(n => n.Status == "OPERANDO" || n.Status == "BERÇO (JÁ ATRACOU)").ToList();
    
    if (atracados.Count == 0)
    {
        Console.WriteLine("Nenhum navio operando no momento.");
    }
    else
    {
        foreach (var navio in atracados)
        {
            string statusFormatado = navio.Status == "BERÇO (JÁ ATRACOU)" ? "🟢 JÁ ATRACOU" : "🚢 OPERANDO";
            string detalheCarga = !string.IsNullOrEmpty(navio.Carga) ? navio.Carga : "MOVIMENTAÇÃO NÃO ESPECIFICADA";
            
            // Exibe os dados focados em operação (Carga/Descarga/Embarque)
            string infoOperacao = detalheCarga;
            if (navio.Descarga != "0" || navio.Embarque != "0")
            {
                infoOperacao += $" (Des: {navio.Descarga}t / Emb: {navio.Embarque}t)";
            }

            Console.WriteLine($"{statusFormatado.PadRight(20)} | {navio.Nome.PadRight(22)} | {navio.Terminal.PadRight(22)} | {infoOperacao}");
        }
    }

    Console.WriteLine(new string('=', 115));
    Console.WriteLine(); // Espaço em branco para respirar a tela

    // -----------------------------------------------------------------------------------
    // SEÇÃO 2: ATRACAÇÕES PROGRAMADAS (Agenda futura e próximas manobras)
    // -----------------------------------------------------------------------------------
    Console.WriteLine("🔮 PRÓXIMAS ATRACAÇÕES PROGRAMADAS (CRONOGRAMA FUTURO)");
    Console.WriteLine(new string('-', 115));
    Console.WriteLine($"{"SITUAÇÃO".PadRight(20)} | {"NAVIO".PadRight(22)} | {"PREVISÃO CHEGADA".PadRight(17)} | {"ESTIMA-SE IR PARA".PadRight(22)} | {"EVENTO"}");
    Console.WriteLine(new string('-', 115));

    // Filtra quem está apenas como programado e ordena por data futura
    var programados = navios.Where(n => n.Status == "PROGRAMADO")
                            .OrderBy(n => n.DataPrevisao ?? DateTime.MaxValue)
                            .ToList();

    if (programados.Count == 0)
    {
        Console.WriteLine("Nenhuma programação futura encontrada para os próximos dias.");
    }
    else
    {
        foreach (var navio in programados)
        {
            string statusFormatado = "🔮 PROGRAMADO";
            string dataPrevisao = navio.DataPrevisao.HasValue ? navio.DataPrevisao.Value.ToString("dd/MM HH:mm") : "---";
            string evento = !string.IsNullOrEmpty(navio.Evento) ? navio.Evento : "ATRACACAO";

            Console.WriteLine($"{statusFormatado.PadRight(20)} | {navio.Nome.PadRight(22)} | {dataPrevisao.PadRight(17)} | {navio.Terminal.PadRight(22)} | {evento}");
        }
    }

    Console.WriteLine(new string('=', 115));
}
    }
}