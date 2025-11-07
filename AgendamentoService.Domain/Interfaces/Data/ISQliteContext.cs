using System.Data;

namespace AgendamentoService.Domain.Interfaces.Data
{
    public interface ISQliteContext
    {
        IDbConnection CreateConnection();
    }
}
