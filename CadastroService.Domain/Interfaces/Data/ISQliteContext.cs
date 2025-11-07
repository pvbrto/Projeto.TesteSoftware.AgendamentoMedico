using System.Data;

namespace CadastroService.Domain.Interfaces.Data
{
    public interface ISQliteContext
    {
        IDbConnection CreateConnection();
    }
}
