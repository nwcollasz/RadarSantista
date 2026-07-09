using Dapper;
using Microsoft.Data.SqlClient;

namespace RadarSantista.src.Repositories
{
    public class InicializadorBanco
    {
        private readonly string _connectionString;

        public InicializadorBanco(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()
        {
            var masterConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
            using (var masterConnection = new SqlConnection(masterConnectionString))
            {
                const string criarBancoQuery = @"
                    IF DB_ID(N'RadarSantista') IS NULL
                    BEGIN
                        CREATE DATABASE [RadarSantista];
                    END;";
                masterConnection.Execute(criarBancoQuery);
            }

            using var connection = new SqlConnection(_connectionString);

            const string queryCriarTabela = @"
                IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NavioHistorico]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[NavioHistorico] (
                        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                        ChaveNegocio NVARCHAR(100) NOT NULL,
                        Tipo NVARCHAR(20) NOT NULL,
                        Nome NVARCHAR(150) NOT NULL,
                        Imo NVARCHAR(50) NULL,
                        Terminal NVARCHAR(150) NULL,
                        Carga NVARCHAR(100) NULL,
                        Descarga NVARCHAR(100) NULL,
                        Embarque NVARCHAR(100) NULL,
                        Evento NVARCHAR(100) NULL,
                        Status NVARCHAR(100) NULL,
                        DataPrevisao DATETIME2 NULL,
                        DataRegistro DATETIME2 NOT NULL DEFAULT SYSDATETIME()
                    );
                END;

                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE object_id = OBJECT_ID(N'[dbo].[NavioHistorico]')
                      AND name = N'IX_NavioHistorico_ChaveNegocio_Tipo_DataRegistro')
                BEGIN
                    CREATE INDEX IX_NavioHistorico_ChaveNegocio_Tipo_DataRegistro
                    ON [dbo].[NavioHistorico] (ChaveNegocio, Tipo, DataRegistro DESC);
                END;";

            connection.Execute(queryCriarTabela);
        }
    }
}
