using CadastroService.Domain.Entities.Especialidades;
using CadastroService.Domain.Entities.Medicos;
using CadastroService.Domain.Interfaces.Data;
using CadastroService.Domain.Interfaces.Repositories;
using Dapper;

namespace CadastroService.Infraestructure.Repositories
{
    public class MedicoRepository : IMedicoRepository
    {
        private readonly ISQliteContext _context;

        public MedicoRepository(ISQliteContext context)
        {
            _context = context;
        }

        public async Task<Medico> Create(Medico medico)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
            INSERT INTO Medicos (Nome, EspecialidadeId, CRM)
            VALUES (@Nome, @EspecialidadeId, @CRM);
            SELECT last_insert_rowid();
        ";

            var id = await connection.ExecuteScalarAsync<int>(sql, medico);
            medico.Id = id;
            return medico;
        }

        public async Task<List<Medico>> GetAll()
        {
            using var connection = _context.CreateConnection();
            string sql = @"
            SELECT m.*, e.Id, e.Nome, e.DataCriacao, e.Ativo
            FROM Medicos m
            INNER JOIN Especialidades e ON m.EspecialidadeId = e.Id
            WHERE m.Ativo = 1 AND e.Ativo = 1;
        ";

            var medicos = await connection.QueryAsync<Medico, Especialidade, Medico>(
                sql,
                (medico, especialidade) =>
                {
                    medico.Especialidade = especialidade;
                    return medico;
                },
                splitOn: "Id"
            );

            return medicos.ToList();
        }

        public async Task<Medico?> GetById(int id)
        {
            using var connection = _context.CreateConnection();
            string sql = @"
            SELECT m.*, e.Id, e.Nome, e.DataCriacao, e.Ativo
            FROM Medicos m
            INNER JOIN Especialidades e ON m.EspecialidadeId = e.Id
            WHERE m.Id = @Id AND m.Ativo = 1;
        ";

            var medico = await connection.QueryAsync<Medico, Especialidade, Medico>(
                sql,
                (m, e) =>
                {
                    m.Especialidade = e;
                    return m;
                },
                new { Id = id },
                splitOn: "Id"
            );

            return medico.FirstOrDefault();
        }

        public async Task<Medico?> Update(Medico medico)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
            UPDATE Medicos
            SET Nome = @Nome,
                EspecialidadeId = @EspecialidadeId,
                CRM = @CRM,
                Ativo = @Ativo
            WHERE Id = @Id;
        ";

            int rows = await connection.ExecuteAsync(sql, medico);
            if (rows == 0) return null;

            return medico;
        }

        public async Task<bool> Delete(int id)
        {
            var medico = await GetById(id);
            if (medico == null) return false;

            medico.Ativo = false;
            var updated = await Update(medico);
            return updated != null;
        }

        public async Task<List<Medico>> GetByEspecialidade(int especialidadeId)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT m.*, e.Id, e.Nome, e.DataCriacao, e.Ativo
                FROM Medicos m
                INNER JOIN Especialidades e ON m.EspecialidadeId = e.Id
                WHERE m.Ativo = 1
                  AND e.Ativo = 1
                  AND m.EspecialidadeId = @EspecialidadeId;
            ";

            var medicos = await connection.QueryAsync<Medico, Especialidade, Medico>(
                sql,
                (medico, especialidade) =>
                {
                    medico.Especialidade = especialidade;
                    return medico;
                },
                new { EspecialidadeId = especialidadeId },
                splitOn: "Id"
            );

            return medicos.ToList();
        }
    }
}
