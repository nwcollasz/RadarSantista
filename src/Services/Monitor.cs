using RadarSantista.src.Repositories;

namespace RadarSantista.src.Services
{
    public class Monitorador
    {
        private readonly INavioRepository _navioRepository;
        private readonly INavioDataSource _navioDataSource;

        public Monitorador(INavioRepository navioRepository, INavioDataSource navioDataSource)
        {
            _navioRepository = navioRepository;
            _navioDataSource = navioDataSource;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine($"[RadarSantista] Executando coleta em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");

                var dadosAtracados = (await _navioDataSource.ObterAtracadosAsync()).ToList();
                var dadosProgramados = (await _navioDataSource.ObterProgramadosAsync()).ToList();

                if (dadosAtracados.Count > 0 || dadosProgramados.Count > 0)
                {
                    _navioRepository.SalvarAtracados(dadosAtracados);
                    _navioRepository.SalvarProgramados(dadosProgramados);
                    Console.WriteLine("Dados persistidos no banco.");
                }
                else
                {
                    Console.WriteLine("Nenhum dado encontrado para persistir.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }
    }
}
