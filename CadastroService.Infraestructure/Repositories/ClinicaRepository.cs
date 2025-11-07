using CadastroService.Domain.Entities.Clinicas;
using CadastroService.Domain.Interfaces.Data;
using CadastroService.Domain.Interfaces.Repositories;
using Dapper;

namespace CadastroService.Infraestructure.Repositories
{
    public class ClinicaRepository(ISQliteContext _context) : IClinicaRepository
    {
        public async Task<Clinica> Create(Clinica clinica)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                INSERT INTO Clinicas (Nome, Endereco)
                VALUES (@Nome, @Endereco);
                SELECT last_insert_rowid();
            ";

            var id = await connection.ExecuteScalarAsync<int>(sql, clinica);
            clinica.Id = id;
            return clinica;
        }

        public async Task<List<Clinica>> GetAll()
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<Clinica>("SELECT * FROM Clinicas WHERE Ativo = 1;");
            return result.ToList();
        }

        public async Task<Clinica> GetById(int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Clinicas WHERE Id = @Id AND Ativo = 1;";
            return await connection.QueryFirstOrDefaultAsync<Clinica>(sql, new { Id = id });
        }

        public async Task<Clinica> Update(Clinica clinica)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                UPDATE Clinicas
                SET Nome = @Nome,
                    Endereco = @Endereco,
                    Ativo = @Ativo
                WHERE Id = @Id;
            ";

            int rows = await connection.ExecuteAsync(sql, clinica);
            if (rows == 0) return null;

            return clinica;
        }

        public async Task<bool> Delete(int id)
        {
            Clinica clinica = await GetById(id);
            if (clinica == null) return false;

            clinica.Ativo = false;
            var clinicaUpdated = await Update(clinica);

            return clinicaUpdated != null;
        }
    }
}
