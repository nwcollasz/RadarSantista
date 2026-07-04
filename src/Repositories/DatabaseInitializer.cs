using Dapper;
using Microsoft.Data.SqlClient;

namespace RadarSantista.src.Repositories
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()
        {
            var masterConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
            using (var masterConnection = new SqlConnection(masterConnectionString))
            {
                const string criarBancoQuery = @"
                    IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RadarSantista')
                    BEGIN
                        CREATE DATABASE RadarSantista;
                    END;";
                masterConnection.Execute(criarBancoQuery);
            }

            using var connection = new SqlConnection(_connectionString);

            const string queryAtracados = @"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HistoricoAtracados]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE HistoricoAtracados (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Nome VARCHAR(150) NOT NULL,
                        Terminal VARCHAR(150),
                        Carga VARCHAR(250),
                        Descarga VARCHAR(50),
                        Embarque VARCHAR(50),
                        Status VARCHAR(50),
                        DataRegistro DATETIME NOT NULL
                    );
                END;";

            const string queryProgramados = @"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HistoricoProgramados]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE HistoricoProgramados (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Nome VARCHAR(150) NOT NULL,
                        Imo VARCHAR(50),
                        Terminal VARCHAR(150),
                        Evento VARCHAR(50),
                        Status VARCHAR(50),
                        DataPrevisao DATETIME,
                        DataRegistro DATETIME NOT NULL
                    );
                END;";

            connection.Execute(queryAtracados);
            connection.Execute(queryProgramados);
        }
    }
}
