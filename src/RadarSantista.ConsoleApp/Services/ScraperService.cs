using System.Globalization;
using HtmlAgilityPack;
using RadarSantista.ConsoleApp.Models;
using RadarSantista.ConsoleApp.Utils;

namespace RadarSantista.ConsoleApp.Services
{
    public class ScraperService
    {
        private readonly HttpClient _client;
        private const int MAX_TENTATIVAS = 3;

        public ScraperService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Navio>> ObterAtracados()
{
    var resultado = new List<Navio>();
    string url = "https://www.portodesantos.com.br/informacoes-operacionais/operacoes-portuarias/navegacao-e-movimento-de-navios/atracados-porto-terminais/";

    var doc = await BaixarHtmlComRetry(url);
    if (doc == null) return resultado;

    var tabelas = doc.DocumentNode.SelectNodes("//table");
    if (tabelas == null) return resultado;

    string ultimoTerminalValido = string.Empty;

    foreach (var tabela in tabelas)
    {
        var linhas = tabela.SelectNodes(".//tr");
        if (linhas == null) continue;

        foreach (var linha in linhas)
        {
            var colunas = linha.SelectNodes(".//td");
            if (colunas != null && colunas.Count >= 9)
            {
                string localBruto = TextHelper.Limpar(colunas[0].InnerText);
                string nomeNavio = TextHelper.Limpar(colunas[1].InnerText);
                string carga = TextHelper.Limpar(colunas[6].InnerText);
                string descarga = TextHelper.Limpar(colunas[7].InnerText);
                string embarque = TextHelper.Limpar(colunas[8].InnerText);

                if (localBruto.Contains("LOCAL") || nomeNavio.Contains("SHIP")) 
                    continue;
                if (string.IsNullOrEmpty(nomeNavio))
                    continue;
                if (!string.IsNullOrEmpty(localBruto))
                {
                    ultimoTerminalValido = localBruto;
                }
                else if (!string.IsNullOrEmpty(ultimoTerminalValido))
                {
                    localBruto = ultimoTerminalValido;
                }

                resultado.Add(new Navio 
                {
                    Nome = nomeNavio, 
                    Terminal = localBruto,
                    Carga = carga, 
                    Descarga = descarga, 
                    Embarque = embarque, 
                    Status = "OPERANDO"
                });
            }
        }
    }
    return resultado;
}

        public async Task<List<Navio>> ObterProgramados()
        {
            var resultado = new List<Navio>();
            string url = "https://www.portodesantos.com.br/informacoes-operacionais/operacoes-portuarias/navegacao-e-movimento-de-navios/atracacoes-programadas/";
            DateTime agora = DateTime.Now;

            var doc = await BaixarHtmlComRetry(url);
            if (doc == null) return resultado;

            var tabelas = doc.DocumentNode.SelectNodes("//table");
            if (tabelas == null) return resultado;

            foreach (var tabela in tabelas)
            {
                var linhas = tabela.SelectNodes(".//tr");
                if (linhas == null) continue;

                foreach (var linha in linhas)
                {
                    var colunas = linha.SelectNodes(".//td");
                    if (colunas != null && colunas.Count >= 8)
                    {
                        string dataStr = TextHelper.Limpar(colunas[0].InnerText);
                        string horaStrRaw = TextHelper.Limpar(colunas[1].InnerText); 
                        string local = TextHelper.Limpar(colunas[3].InnerText);    
                        string nomeNavio = TextHelper.Limpar(colunas[4].InnerText);    
                        string imo = TextHelper.Limpar(colunas[5].InnerText);      
                        string evento = TextHelper.Limpar(colunas[7].InnerText);   

                        if (dataStr.Contains("DATA") || string.IsNullOrEmpty(nomeNavio) || nomeNavio.Contains("SHIP")) 
                            continue;

                        string horaStr = horaStrRaw.Split('/')[0].Trim();
                        string dataComAno = dataStr.Length <= 5 ? $"{dataStr}/{agora.Year}" : dataStr;

                        if (DateTime.TryParseExact($"{dataComAno} {horaStr}", new[] { "dd/MM/yyyy HH:mm", "dd/MM/yy HH:mm" }, 
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataProgramada))
                        {
                            if (dataProgramada.Date < agora.Date) continue;

                            resultado.Add(new Navio 
                            {
                                Nome = nomeNavio, Imo = imo, Terminal = local, Evento = evento, DataPrevisao = dataProgramada, Status = "PROGRAMADO"
                            });
                        }
                    }
                }
            }
            return resultado;
        }

        private async Task<HtmlDocument> BaixarHtmlComRetry(string url)
        {
            for (int tentativa = 1; tentativa <= MAX_TENTATIVAS; tentativa++)
            {
                try
                {
                    string html = await _client.GetStringAsync(url);
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    return doc;
                }
                catch
                {
                    if (tentativa == MAX_TENTATIVAS) return null;
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            }
            return null;
        }
    }
}