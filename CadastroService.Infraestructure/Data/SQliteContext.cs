using CadastroService.Domain.Interfaces.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace CadastroService.Infraestructure.Data
{
    public class SQliteContext : ISQliteContext
    {
        private readonly string _connectionString;

        public SQliteContext(IConfiguration config)
        {
            // Caminho absoluto da pasta Infraestructure/Data
            var baseDir = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            var dataPath = Path.Combine(baseDir, "CadastroService.Infraestructure", "Data", "cadastroService.db");

            // Cria a pasta se não existir
            var dataFolder = Path.GetDirectoryName(dataPath)!;
            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);

            _connectionString = string.Format(config.GetConnectionString("CadastroServiceConnection"), dataPath);
            EnsureDatabase();
        }

        public IDbConnection CreateConnection()
            => new SqliteConnection(_connectionString);

        private void EnsureDatabase()
        {
            using var connection = CreateConnection();
            connection.Open();

            // Criar tabela Pacientes
            ExecuteSQL(connection, @"
            CREATE TABLE IF NOT EXISTS Pacientes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nome TEXT NOT NULL,
                Email TEXT,
                Phone TEXT,
                DataNascimento TEXT,
                DataCriacao TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                Ativo INTEGER NOT NULL DEFAULT 1
            );");

            // Criar tabela Medicos
            ExecuteSQL(connection, @"
            CREATE TABLE IF NOT EXISTS Medicos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nome TEXT NOT NULL,
                EspecialidadeId INTEGER,
                Crm TEXT,
                DataCriacao TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                Ativo INTEGER NOT NULL DEFAULT 1,
                FOREIGN KEY (EspecialidadeId) REFERENCES Especialidades(Id) ON DELETE SET NULL
            )");

            // Criar tabela Especialidades
            ExecuteSQL(connection, @"
            CREATE TABLE IF NOT EXISTS Especialidades (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nome TEXT NOT NULL,
                DataCriacao TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                Ativo INTEGER NOT NULL DEFAULT 1
            );");

            // Criar tabela Clinicas
            ExecuteSQL(connection, @"
            CREATE TABLE IF NOT EXISTS Clinicas (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nome TEXT NOT NULL,
                Endereco TEXT,
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
