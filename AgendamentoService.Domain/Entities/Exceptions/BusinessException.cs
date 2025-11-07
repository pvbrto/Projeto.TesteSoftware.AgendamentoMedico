using System.Net;

namespace AgendamentoService.Domain.Entities.Exceptions
{
    public class BusinessException : Exception
    {
        public HttpStatusCode Code { get; set; }

        public BusinessException(HttpStatusCode code, string message) : base(message)
        {
            Code = code;
        }
    }
}
