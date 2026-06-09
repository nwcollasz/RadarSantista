using Microsoft.Data.Sqlite;
using Dapper;
using RadarSantista.ConsoleApp.Models;

namespace RadarSantista.ConsoleApp.Repositories
{
    public class DatabaseRepository
    {
        private readonly string _connectionString;

        public DatabaseRepository(string nomeBanco = "radar_porto.db")
        {
            _connectionString = $"Data Source={nomeBanco}";
            InicializarBanco();
        }

        private void InicializarBanco()
        {
            using var connection = new SqliteConnection(_connectionString);
            
            string queryAtracados = @"
                CREATE TABLE IF NOT EXISTS HistoricoAtracados (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nome TEXT NOT NULL,
                    Terminal TEXT,
                    Carga TEXT,
                    Descarga TEXT,
                    Embarque TEXT,
                    Status TEXT,
                    DataRegistro TEXT NOT NULL
                );";

            string queryProgramados = @"
                CREATE TABLE IF NOT EXISTS HistoricoProgramados (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nome TEXT NOT NULL,
                    Imo TEXT,
                    Terminal TEXT,
                    Evento TEXT,
                    Status TEXT,
                    DataPrevisao TEXT,
                    DataRegistro TEXT NOT NULL
                );";
                
            connection.Execute(queryAtracados);
            connection.Execute(queryProgramados);
        }

        public void SalvarAtracados(List<Navio> navios)
        {
            if (navios == null || navios.Count == 0) return;

            using var connection = new SqliteConnection(_connectionString);
            
            string query = @"
                INSERT INTO HistoricoAtracados (Nome, Terminal, Carga, Descarga, Embarque, Status, DataRegistro)
                VALUES (@Nome, @Terminal, @Carga, @Descarga, @Embarque, @Status, @DataRegistro);";

            connection.Execute(query, navios);
        }

        public void SalvarProgramados(List<Navio> navios)
        {
            if (navios == null || navios.Count == 0) return;

            using var connection = new SqliteConnection(_connectionString);
            
            string query = @"
                INSERT INTO HistoricoProgramados (Nome, Imo, Terminal, Evento, Status, DataPrevisao, DataRegistro)
                VALUES (@Nome, @Imo, @Terminal, @Evento, @Status, @DataPrevisao, @DataRegistro);";

            connection.Execute(query, navios);
        }
    }
}