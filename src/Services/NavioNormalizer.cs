using RadarSantista.src.Models;
using RadarSantista.src.Utils;

namespace RadarSantista.src.Services
{
    public class NavioNormalizer
    {
        public Navio NormalizarAtracado(string nome, string terminal, string carga, string descarga, string embarque)
        {
            return new Navio
            {
                Nome = NormalizarTexto(nome),
                Terminal = NormalizarTexto(terminal),
                Carga = NormalizarTexto(carga),
                Descarga = NormalizarTexto(descarga),
                Embarque = NormalizarTexto(embarque),
                Status = "OPERANDO"
            };
        }

        public Navio? NormalizarProgramado(string dataStr, string horaStrRaw, string local, string nomeNavio, string imo, string evento, DateTime agora)
        {
            var nome = NormalizarTexto(nomeNavio);
            if (string.IsNullOrWhiteSpace(nome))
            {
                return null;
            }

            var horaStr = horaStrRaw.Split('/')[0].Trim();
            var dataComAno = dataStr.Length <= 5 ? $"{dataStr}/{agora.Year}" : dataStr;

            if (!DateTime.TryParseExact(
                    $"{dataComAno} {horaStr}",
                    new[] { "dd/MM/yyyy HH:mm", "dd/MM/yy HH:mm" },
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var dataProgramada))
            {
                return null;
            }

            if (dataProgramada.Date < agora.Date)
            {
                return null;
            }

            return new Navio
            {
                Nome = nome,
                Imo = NormalizarTexto(imo),
                Terminal = NormalizarTexto(local),
                Evento = NormalizarTexto(evento),
                DataPrevisao = dataProgramada,
                Status = "PROGRAMADO"
            };
        }

        public string NormalizarTexto(string texto)
        {
            return TextHelper.Limpar(texto);
        }
    }
}
