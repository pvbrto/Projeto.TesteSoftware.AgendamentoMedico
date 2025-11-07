using CadastroService.Domain.Entities.Pacientes;
using CadastroService.Domain.Interfaces.Data;
using CadastroService.Domain.Interfaces.Repositories;
using Dapper;

namespace CadastroService.Infraestructure.Repositories
{
    public class PacienteRepository(ISQliteContext _context) : IPacienteRepository
    {
        public async Task<Paciente> Create(Paciente paciente)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
            INSERT INTO Pacientes (Nome, Email, Phone, DataNascimento)
            VALUES (@Nome, @Email, @Phone, @DataNascimento);
            SELECT last_insert_rowid();
        ";

            var id = await connection.ExecuteScalarAsync<int>(sql, paciente);
            paciente.Id = id;
            return paciente;
        }

        public async Task<List<Paciente>> GetAll()
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<Paciente>("SELECT * FROM Pacientes WHERE Ativo = 1;");
            return result.ToList();
        }

        public async Task<Paciente?> GetById(int id)
        {
            using var connection = _context.CreateConnection();
            string sql = "SELECT * FROM Pacientes WHERE Id = @Id AND Ativo = 1;";
            return await connection.QueryFirstOrDefaultAsync<Paciente>(sql, new { Id = id });
        }

        public async Task<Paciente?> Update(Paciente paciente)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
            UPDATE Pacientes
            SET Nome = @Nome,
                Email = @Email,
                Phone = @Phone,
                DataNascimento = @DataNascimento,
                Ativo = @Ativo
            WHERE Id = @Id;
        ";

            int rows = await connection.ExecuteAsync(sql, paciente);
            if (rows == 0) return null;

            return paciente;
        }

        public async Task<bool> Delete(int id)
        {
            var paciente = await GetById(id);
            if (paciente == null) return false;

            paciente.Ativo = false;
            var updated = await Update(paciente);
            return updated != null;
        }
    }
}
