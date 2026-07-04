using HtmlAgilityPack;
using RadarSantista.src.Models;

namespace RadarSantista.src.Services
{
    public class ScraperService : INavioDataSource
    {
        private readonly HttpClient _client;
        private readonly NavioNormalizer _normalizer;
        private const int MAX_TENTATIVAS = 3;

        public ScraperService(HttpClient client)
        {
            _client = client;
            _normalizer = new NavioNormalizer();
        }

        public async Task<IReadOnlyList<Navio>> ObterAtracadosAsync(CancellationToken cancellationToken = default)
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
                string localBruto = _normalizer.NormalizarTexto(colunas[0].InnerText);
                string nomeNavio = _normalizer.NormalizarTexto(colunas[1].InnerText);
                string carga = _normalizer.NormalizarTexto(colunas[6].InnerText);
                string descarga = _normalizer.NormalizarTexto(colunas[7].InnerText);
                string embarque = _normalizer.NormalizarTexto(colunas[8].InnerText);

                if (localBruto.Contains("LOCAL") || nomeNavio.Contains("SHIP")) 
                    continue;
                if (string.IsNullOrWhiteSpace(nomeNavio))
                    continue;
                if (!string.IsNullOrEmpty(localBruto))
                {
                    ultimoTerminalValido = localBruto;
                }
                else if (!string.IsNullOrEmpty(ultimoTerminalValido))
                {
                    localBruto = ultimoTerminalValido;
                }

                resultado.Add(_normalizer.NormalizarAtracado(nomeNavio, localBruto, carga, descarga, embarque));
            }
        }
    }
    return resultado;
}

        public async Task<IReadOnlyList<Navio>> ObterProgramadosAsync(CancellationToken cancellationToken = default)
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
                        string dataStr = _normalizer.NormalizarTexto(colunas[0].InnerText);
                        string horaStrRaw = _normalizer.NormalizarTexto(colunas[1].InnerText); 
                        string local = _normalizer.NormalizarTexto(colunas[3].InnerText);    
                        string nomeNavio = _normalizer.NormalizarTexto(colunas[4].InnerText);    
                        string imo = _normalizer.NormalizarTexto(colunas[5].InnerText);      
                        string evento = _normalizer.NormalizarTexto(colunas[7].InnerText);   

                        if (dataStr.Contains("DATA") || string.IsNullOrWhiteSpace(nomeNavio) || nomeNavio.Contains("SHIP")) 
                            continue;

                        var navioProgramado = _normalizer.NormalizarProgramado(dataStr, horaStrRaw, local, nomeNavio, imo, evento, agora);
                        if (navioProgramado != null)
                        {
                            resultado.Add(navioProgramado);
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