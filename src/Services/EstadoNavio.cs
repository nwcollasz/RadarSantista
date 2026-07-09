using RadarSantista.src.Models;

namespace RadarSantista.src.Services
{
    public class EstadoNavio
    {
        public bool DevePersistirAtracado(Navio novoNavio, Navio? ultimoRegistro)
        {
            if (ultimoRegistro is null)
            {
                return true;
            }

            var snapshotAtual = NavioEstadoSnapshot.Criar(novoNavio);
            var snapshotAnterior = NavioEstadoSnapshot.Criar(ultimoRegistro);

            return !TemMesmoEstadoAtracado(snapshotAtual, snapshotAnterior);
        }

        public bool DevePersistirProgramado(Navio novoNavio, Navio? ultimoRegistro)
        {
            if (ultimoRegistro is null)
            {
                return true;
            }

            var snapshotAtual = NavioEstadoSnapshot.Criar(novoNavio);
            var snapshotAnterior = NavioEstadoSnapshot.Criar(ultimoRegistro);

            return !TemMesmoEstadoProgramado(snapshotAtual, snapshotAnterior);
        }

        private static bool TemMesmoEstadoAtracado(NavioEstadoSnapshot atual, NavioEstadoSnapshot anterior)
        {
            return string.Equals(atual.Terminal, anterior.Terminal, StringComparison.OrdinalIgnoreCase)
                && string.Equals(atual.Carga, anterior.Carga, StringComparison.OrdinalIgnoreCase)
                && string.Equals(atual.Descarga, anterior.Descarga, StringComparison.OrdinalIgnoreCase)
                && string.Equals(atual.Embarque, anterior.Embarque, StringComparison.OrdinalIgnoreCase)
                && string.Equals(atual.Status, anterior.Status, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TemMesmoEstadoProgramado(NavioEstadoSnapshot atual, NavioEstadoSnapshot anterior)
        {
            if (atual.DataPrevisao.HasValue && anterior.DataPrevisao.HasValue && atual.DataPrevisao.Value != anterior.DataPrevisao.Value)
            {
                return false;
            }

            return string.Equals(atual.Terminal, anterior.Terminal, StringComparison.OrdinalIgnoreCase)
                && string.Equals(atual.Evento, anterior.Evento, StringComparison.OrdinalIgnoreCase)
                && string.Equals(atual.Status, anterior.Status, StringComparison.OrdinalIgnoreCase);
        }
    }
}
