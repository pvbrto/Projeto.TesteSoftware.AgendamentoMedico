using AgendamentoService.Domain.Entities.Consultas;
using AgendamentoService.Domain.Interfaces.Data;
using AgendamentoService.Domain.Interfaces.Repositories;
using Dapper;

namespace AgendamentoService.Infraestructure.Repositories
{
    public class ConsultaRepository(ISQliteContext _context) : IConsultaRepository
    {
        public async Task<List<Consulta>> GetAll()
        {
            using var connection = _context.CreateConnection();
            string sql = @"
            SELECT *
            FROM Consultas
            WHERE Ativo = 1;
        ";

            var consultas = await connection.QueryAsync<Consulta>(sql);
            return consultas.ToList();
        }

        public async Task<Consulta?> GetById(int id)
        {
            using var connection = _context.CreateConnection();
            string sql = @"
            SELECT *
            FROM Consultas
            WHERE Id = @Id AND Ativo = 1;
        ";

            var consulta = await connection.QueryFirstOrDefaultAsync<Consulta>(sql, new { Id = id });
            return consulta;
        }

        public async Task<Consulta> Create(Consulta consulta)
        {
            using var connection = _context.CreateConnection();
            string sql = @"
            INSERT INTO Consultas (PacienteId, MedicoId, ClinicaId, DataHora, Observacoes, Status)
            VALUES (@PacienteId, @MedicoId, @ClinicaId, @DataHora, @Observacoes, @Status);
            SELECT last_insert_rowid();
        ";

            var id = await connection.ExecuteScalarAsync<int>(sql, consulta);
            consulta.Id = id;
            return consulta;
        }

        public async Task<Consulta?> Update(Consulta consulta)
        {
            using var connection = _context.CreateConnection();
            string sql = @"
            UPDATE Consultas
            SET PacienteId = @PacienteId,
                MedicoId = @MedicoId,
                ClinicaId = @ClinicaId,
                DataHora = @DataHora,
                Observacoes = @Observacoes,
                Status = @Status,
                Ativo = @Ativo
            WHERE Id = @Id;
        ";

            int rows = await connection.ExecuteAsync(sql, consulta);
            return rows > 0 ? consulta : null;
        }

        public async Task<bool> Delete(int id)
        {
            var consulta = await GetById(id);
            if (consulta == null) return false;

            consulta.Ativo = false;
            var updated = await Update(consulta);
            return updated != null;
        }

        public async Task<Consulta?> GetConsultaNoHorario(int medicoId, int clinicaId, DateTime dataHora)
        {
            using var connection = _context.CreateConnection();

            // Define o intervalo de 30 minutos antes e depois
            var inicio = dataHora.AddMinutes(-30);
            var fim = dataHora.AddMinutes(30);

            string sql = @"
                SELECT *
                FROM Consultas
                WHERE MedicoId = @MedicoId
                  AND ClinicaId = @ClinicaId
                  AND Ativo = 1
                  AND datetime(DataHora) BETWEEN datetime(@Inicio) AND datetime(@Fim);
            ";

            var consulta = await connection.QueryFirstOrDefaultAsync<Consulta>(sql, new
            {
                MedicoId = medicoId,
                ClinicaId = clinicaId,
                Inicio = inicio,
                Fim = fim
            });

            return consulta;
        }
    }
}
