using RadarSantista.src.Models;

namespace RadarSantista.src.Repositories
{
    public interface INavioRepository
    {
        void SalvarAtracados(List<Navio> navios);
        void SalvarProgramados(List<Navio> navios);
    }
}
