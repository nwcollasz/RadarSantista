using RadarSantista.src.Models;

namespace RadarSantista.src.Services
{
    public interface INavioDataSource
    {
        Task<IReadOnlyList<Navio>> ObterAtracadosAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Navio>> ObterProgramadosAsync(CancellationToken cancellationToken = default);
    }
}
