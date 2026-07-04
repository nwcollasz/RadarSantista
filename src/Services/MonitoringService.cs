using RadarSantista.src.Models;
using RadarSantista.src.Repositories;

namespace RadarSantista.src.Services
{
    public class MonitoringService
    {
        private readonly INavioRepository _navioRepository;
        private readonly INavioDataSource _navioDataSource;
        private readonly IConsoleOutput _consoleOutput;
        private readonly List<Navio> _cacheConsolidado = new();

        public MonitoringService(INavioRepository navioRepository, INavioDataSource navioDataSource, IConsoleOutput consoleOutput)
        {
            _navioRepository = navioRepository;
            _navioDataSource = navioDataSource;
            _consoleOutput = consoleOutput;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _consoleOutput.Clear();
                _consoleOutput.WriteLine($"[RadarSantista] Executado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                _consoleOutput.WriteLine(new string('=', 102));

                _consoleOutput.WriteLine("Coletando dados do Porto:");
                var dadosAtracados = (await _navioDataSource.ObterAtracadosAsync()).ToList();
                var dadosProgramados = (await _navioDataSource.ObterProgramadosAsync()).ToList();

                if (dadosAtracados.Count > 0 || dadosProgramados.Count > 0)
                {
                    _cacheConsolidado.Clear();
                    _cacheConsolidado.AddRange(DataEngine.Consolidar(dadosAtracados, dadosProgramados));

                    _navioRepository.SalvarAtracados(dadosAtracados);
                    _navioRepository.SalvarProgramados(dadosProgramados);
                }
                else if (_cacheConsolidado.Count > 0)
                {
                    _consoleOutput.WriteLine("Falha ao atualizar.");
                }

                ConsoleVisualService.ExibirPainel(_cacheConsolidado);

                _consoleOutput.WriteLine("\nAguarde 1 min para o próximo monitoramento:");
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }
    }
}
