using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Dapper;
using RadarSantista.src.Models;
using RadarSantista.src.Services;

namespace RadarSantista.src.Repositories
{
    public class DatabaseRepository : INavioRepository
    {
        private readonly string _connectionString;
        private readonly NavioStateService _navioStateService;

        public DatabaseRepository(string connectionString = null)
        {
            _navioStateService = new NavioStateService();
            _connectionString = connectionString ?? 
                "Server=(localdb)\\MSSQLLocalDB;Database=RadarSantista;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public void SalvarAtracados(List<Navio> navios)
        {
            if (navios == null || navios.Count == 0) return;

            using var connection = new SqlConnection(_connectionString);

            var naviosUnicos = new List<Navio>();
            foreach (var n in navios)
            {
                if (!naviosUnicos.Exists(x => (x.Nome ?? "").Trim().Equals((n.Nome ?? "").Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    naviosUnicos.Add(n);
                }
            }
            
            string queryUltimoStatus = @"
                SELECT TOP 1 Terminal, Carga, Descarga, Embarque, Status 
                FROM HistoricoAtracados 
                WHERE Nome = @Nome 
                ORDER BY Id DESC;";

            string queryInsert = @"
                INSERT INTO HistoricoAtracados (Nome, Terminal, Carga, Descarga, Embarque, Status, DataRegistro)
                VALUES (@Nome, @Terminal, @Carga, @Descarga, @Embarque, @Status, @DataRegistro);";

            foreach (var navio in naviosUnicos)
            {
                var ultimoRegistro = connection.QueryFirstOrDefault<Navio>(queryUltimoStatus, new { Nome = navio.Nome });

                if (ultimoRegistro == null)
                {
                    connection.Execute(queryInsert, navio);
                    continue;
                }

                if (_navioStateService.DevePersistirAtracado(navio, ultimoRegistro))
                {
                    connection.Execute(queryInsert, navio);
                }
            }
        }

        public void SalvarProgramados(List<Navio> navios)
        {
            if (navios == null || navios.Count == 0) return;

            using var connection = new SqlConnection(_connectionString);

            var naviosUnicos = new List<Navio>();
            foreach (var n in navios)
            {
                if (!naviosUnicos.Exists(x => (x.Nome ?? "").Trim().Equals((n.Nome ?? "").Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    naviosUnicos.Add(n);
                }
            }
            
            string queryUltimoStatus = @"
                SELECT TOP 1 Terminal, Evento, Status, DataPrevisao 
                FROM HistoricoProgramados 
                WHERE Nome = @Nome 
                ORDER BY Id DESC;";

            string queryInsert = @"
                INSERT INTO HistoricoProgramados (Nome, Imo, Terminal, Evento, Status, DataPrevisao, DataRegistro)
                VALUES (@Nome, @Imo, @Terminal, @Evento, @Status, @DataPrevisao, @DataRegistro);";

            foreach (var navio in naviosUnicos)
            {
                var ultimoRegistro = connection.QueryFirstOrDefault<Navio>(queryUltimoStatus, new { Nome = navio.Nome });
                
                if (ultimoRegistro == null)
                {
                    connection.Execute(queryInsert, navio);
                    continue;
                }

                if (_navioStateService.DevePersistirProgramado(navio, ultimoRegistro))
                {
                    connection.Execute(queryInsert, navio);
                }
            }
        }
    }
}