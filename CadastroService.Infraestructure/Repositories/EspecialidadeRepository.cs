using CadastroService.Domain.Entities.Especialidades;
using CadastroService.Domain.Interfaces.Data;
using CadastroService.Domain.Interfaces.Repositories;
using Dapper;

namespace CadastroService.Infraestructure.Repositories
{
    public class EspecialidadeRepository : IEspecialidadeRepository
    {
        private readonly ISQliteContext _context;

        public EspecialidadeRepository(ISQliteContext context)
        {
            _context = context;
        }

        public async Task<Especialidade> Create(Especialidade especialidade)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
            INSERT INTO Especialidades (Nome)
            VALUES (@Nome);
            SELECT last_insert_rowid();
        ";

            var id = await connection.ExecuteScalarAsync<int>(sql, especialidade);
            especialidade.Id = id;
            return especialidade;
        }

        public async Task<List<Especialidade>> GetAll()
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<Especialidade>("SELECT * FROM Especialidades WHERE Ativo = 1;");
            return result.ToList();
        }

        public async Task<Especialidade?> GetById(int id)
        {
            using var connection = _context.CreateConnection();
            string sql = "SELECT * FROM Especialidades WHERE Id = @Id AND Ativo = 1;";
            return await connection.QueryFirstOrDefaultAsync<Especialidade>(sql, new { Id = id });
        }

        public async Task<Especialidade?> Update(Especialidade especialidade)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
            UPDATE Especialidades
            SET Nome = @Nome,
                Ativo = @Ativo
            WHERE Id = @Id;
        ";

            int rows = await connection.ExecuteAsync(sql, especialidade);
            if (rows == 0) return null;

            return especialidade;
        }

        public async Task<bool> Delete(int id)
        {
            var especialidade = await GetById(id);
            if (especialidade == null) return false;

            especialidade.Ativo = false;
            var updated = await Update(especialidade);
            return updated != null;
        }
    }

}
