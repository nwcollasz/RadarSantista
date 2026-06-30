using RadarSantista.ConsoleApp.Models;

namespace RadarSantista.ConsoleApp.Services
{
    public static class ConsoleVisualService
    {
        public static void ExibirPainel(List<Navio> navios)
        {
            Console.Clear();
            Console.WriteLine($"[RadarSantista] Atualizado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine(new string('=', 102));

            Console.WriteLine("NAVIOS ATRACADOS");
            Console.WriteLine(new string('-', 102));
            Console.WriteLine($"{"SITUAÇÃO",-20} | {"NAVIO",-22} | {"TERMINAL",-22} | {"MOVIMENTAÇÃO / CARGA"}");
            Console.WriteLine(new string('-', 102));

            var atracados = navios.Where(n => n.Status == "OPERANDO" || n.Status == "BERÇO (JÁ ATRACOU)").ToList();
            
            if (atracados.Count == 0)
            {
                Console.WriteLine("Nenhum navio operando no momento.");
            }
            else
            {
                foreach (var navio in atracados)
                {
                    string statusFormatado = navio.Status == "BERÇO" ? "JÁ ATRACOU" : "OPERANDO";
                    string detalheCarga = !string.IsNullOrEmpty(navio.Carga) ? navio.Carga : "MOVIMENTAÇÃO NÃO ESPECIFICADA";
                    
                    string infoOperacao = detalheCarga;
                    if (navio.Descarga != "0" || navio.Embarque != "0")
                    {
                        infoOperacao += $" (Des: {navio.Descarga}t / Emb: {navio.Embarque}t)";
                    }

                    Console.WriteLine($"{statusFormatado,-20} | {navio.Nome,-22} | {navio.Terminal,-22} | {infoOperacao}");
                }
            }

            Console.WriteLine(new string('=', 102));
            Console.WriteLine(); 

            Console.WriteLine("PRÓXIMAS ATRACAÇÕES PROGRAMADAS");
            Console.WriteLine(new string('-', 102));
            Console.WriteLine($"{"SITUAÇÃO",-20} | {"NAVIO",-22} | {"PREVISÃO CHEGADA",-17} | {"ESTIMA-SE IR PARA",-22} | {"EVENTO"}");
            Console.WriteLine(new string('-', 102));

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
                    string statusFormatado = "PROGRAMADO";
                    string dataPrevisao = navio.DataPrevisao.HasValue ? navio.DataPrevisao.Value.ToString("dd/MM HH:mm") : "---";
                    string evento = !string.IsNullOrEmpty(navio.Evento) ? navio.Evento : "ATRACACAO";

                    Console.WriteLine($"{statusFormatado,-20} | {navio.Nome,-22} | {dataPrevisao,-17} | {navio.Terminal,-22} | {evento}");
                }
            }

            Console.WriteLine(new string('=', 102));
        }
    }
}