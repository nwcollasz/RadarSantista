using RadarSantista.src.Models;
using RadarSantista.src.Utils;

namespace RadarSantista.src.Models
{
    public sealed class NavioEstadoSnapshot
    {
        public string Nome { get; }
        public string Terminal { get; }
        public string Carga { get; }
        public string Descarga { get; }
        public string Embarque { get; }
        public string Evento { get; }
        public string Status { get; }
        public DateTime? DataPrevisao { get; }

        private NavioEstadoSnapshot(
            string nome,
            string terminal,
            string carga,
            string descarga,
            string embarque,
            string evento,
            string status,
            DateTime? dataPrevisao)
        {
            Nome = nome;
            Terminal = terminal;
            Carga = carga;
            Descarga = descarga;
            Embarque = embarque;
            Evento = evento;
            Status = status;
            DataPrevisao = dataPrevisao;
        }

        public static NavioEstadoSnapshot Criar(Navio navio)
        {
            ArgumentNullException.ThrowIfNull(navio);

            return new NavioEstadoSnapshot(
                Normalizar(navio.Nome),
                Normalizar(navio.Terminal),
                Normalizar(navio.Carga),
                Normalizar(navio.Descarga),
                Normalizar(navio.Embarque),
                Normalizar(navio.Evento),
                Normalizar(navio.Status),
                navio.DataPrevisao?.Date);
        }

        private static string Normalizar(string? valor)
        {
            return TextHelper.Limpar(valor ?? string.Empty);
        }
    }
}
