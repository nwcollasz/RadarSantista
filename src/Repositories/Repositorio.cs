using Microsoft.Data.SqlClient;
using Dapper;
using RadarSantista.src.Models;
using RadarSantista.src.Services;

namespace RadarSantista.src.Repositories
{
    public class Repositorio : INavioRepository
    {
        private readonly string _connectionString;
        private readonly EstadoNavio _navioStateService;

        public Repositorio(string? connectionString = null)
        {
            _navioStateService = new EstadoNavio();
            _connectionString = connectionString ??
                "Server=(localdb)\\MSSQLLocalDB;Database=RadarSantista;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public void SalvarAtracados(List<Navio> navios)
        {
            SalvarHistorico(navios, "ATRACADO");
        }

        public void SalvarProgramados(List<Navio> navios)
        {
            SalvarHistorico(navios, "PROGRAMADO");
        }

        private void SalvarHistorico(List<Navio> navios, string tipo)
        {
            if (navios == null || navios.Count == 0)
            {
                return;
            }

            using var connection = new SqlConnection(_connectionString);

            var naviosUnicos = navios
                .Where(n => n != null)
                .GroupBy(n => n.ObterChaveNegocio(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.Last())
                .ToList();

            const string queryUltimoRegistro = @"
                SELECT TOP 1 Nome, Imo, Terminal, Carga, Descarga, Embarque, Evento, Status, DataPrevisao
                FROM NavioHistorico
                WHERE ChaveNegocio = @ChaveNegocio AND Tipo = @Tipo
                ORDER BY Id DESC;";

            const string queryInsert = @"
                INSERT INTO NavioHistorico (
                    ChaveNegocio, Tipo, Nome, Imo, Terminal, Carga, Descarga, Embarque, Evento, Status, DataPrevisao, DataRegistro)
                VALUES (
                    @ChaveNegocio, @Tipo, @Nome, @Imo, @Terminal, @Carga, @Descarga, @Embarque, @Evento, @Status, @DataPrevisao, @DataRegistro);";

            foreach (var navio in naviosUnicos)
            {
                var chaveNegocio = navio.ObterChaveNegocio();
                var ultimoRegistro = connection.QueryFirstOrDefault<Navio>(queryUltimoRegistro, new { ChaveNegocio = chaveNegocio, Tipo = tipo });

                if (ultimoRegistro == null)
                {
                    connection.Execute(queryInsert, new
                    {
                        ChaveNegocio = chaveNegocio,
                        Tipo = tipo,
                        Nome = navio.Nome,
                        Imo = navio.Imo,
                        Terminal = navio.Terminal,
                        Carga = navio.Carga,
                        Descarga = navio.Descarga,
                        Embarque = navio.Embarque,
                        Evento = navio.Evento,
                        Status = navio.Status,
                        DataPrevisao = navio.DataPrevisao,
                        DataRegistro = navio.DataRegistro
                    });
                    continue;
                }

                var devePersistir = tipo == "ATRACADO"
                    ? _navioStateService.DevePersistirAtracado(navio, ultimoRegistro)
                    : _navioStateService.DevePersistirProgramado(navio, ultimoRegistro);

                if (devePersistir)
                {
                    connection.Execute(queryInsert, new
                    {
                        ChaveNegocio = chaveNegocio,
                        Tipo = tipo,
                        Nome = navio.Nome,
                        Imo = navio.Imo,
                        Terminal = navio.Terminal,
                        Carga = navio.Carga,
                        Descarga = navio.Descarga,
                        Embarque = navio.Embarque,
                        Evento = navio.Evento,
                        Status = navio.Status,
                        DataPrevisao = navio.DataPrevisao,
                        DataRegistro = navio.DataRegistro
                    });
                }
            }
        }
    }
}