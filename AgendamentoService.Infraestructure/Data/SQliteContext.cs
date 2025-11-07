using AgendamentoService.Domain.Interfaces.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace AgendamentoService.Infraestructure.Data
{
    public class SQliteContext : ISQliteContext
    {
        private readonly string _connectionString;

        public SQliteContext(IConfiguration config)
        {
            // Caminho absoluto da pasta Infraestructure/Data
            var baseDir = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            var dataPath = Path.Combine(baseDir, "AgendamentoService.Infraestructure", "Data", "agendamentoService.db");

            // Cria a pasta se não existir
            var dataFolder = Path.GetDirectoryName(dataPath)!;
            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);

            _connectionString = string.Format(config.GetConnectionString("AgendamentoServiceConnection"), dataPath);
            EnsureDatabase();
        }

        public IDbConnection CreateConnection()
            => new SqliteConnection(_connectionString);

        private void EnsureDatabase()
        {
            using var connection = CreateConnection();
            connection.Open();

            // Criar tabela Consultas
            ExecuteSQL(connection, @"
            CREATE TABLE IF NOT EXISTS Consultas (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PacienteId INTEGER NOT NULL,
                MedicoId INTEGER NOT NULL,
                ClinicaId INTEGER NOT NULL,
                DataHora TEXT NOT NULL,
                Observacoes TEXT,
                Status TEXT,
                DataCriacao TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                Ativo INTEGER NOT NULL DEFAULT 1
            );");
        }

        private static void ExecuteSQL(IDbConnection connection, string sql)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
