using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Email;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;

namespace AriesContador.Data.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IConnectionString _connectionString;

        public EmailRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<DataTable> GetLogAsync(CancellationToken cancellationToken = default)
        {
            const string sql = "select nombre,apellido,correo_electronico,correo_copia,asunto,titulo,mensaje,estado,ultimo_envio from usuarios_correo order by mailusuario_id desc";
            var dataAccess = new MySqlDataAccess(_connectionString);
            return dataAccess.QueryTableAsync(sql, new { }, cancellationToken);
        }

        public async Task<bool> InsertAsync(MailMessageLog message, CancellationToken cancellationToken = default)
        {
            const string sql = "insert into usuarios_correo (nombre,apellido,correo_electronico,correo_copia,asunto,titulo,mensaje,estado) "
                               + "VALUES(@FirstName, @LastName, @ToAddress, @CcAddress, @Subject, @Title, @Body, @Estado)";
            var dataAccess = new MySqlDataAccess(_connectionString);
            var estado = message.Sent ? 1 : 2;
            return await dataAccess.ExecuteTextAsync(sql, new
            {
                message.FirstName,
                message.LastName,
                message.ToAddress,
                message.CcAddress,
                message.Subject,
                message.Title,
                message.Body,
                Estado = estado
            }, cancellationToken).ConfigureAwait(false) > 0;
        }
    }
}
