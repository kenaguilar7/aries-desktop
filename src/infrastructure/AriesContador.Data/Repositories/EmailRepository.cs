using System.Data;
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

        public DataTable GetLog()
        {
            const string sql = "select nombre,apellido,correo_electronico,correo_copia,asunto,titulo,mensaje,estado,ultimo_envio from usuarios_correo order by mailusuario_id desc";
            var dataAccess = new MySqlDataAccess(_connectionString);
            return dataAccess.QueryTable(sql, new { });
        }

        public bool Insert(MailMessageLog message)
        {
            const string sql = "insert into usuarios_correo (nombre,apellido,correo_electronico,correo_copia,asunto,titulo,mensaje,estado) "
                               + "VALUES(@FirstName, @LastName, @ToAddress, @CcAddress, @Subject, @Title, @Body, @Estado)";
            var dataAccess = new MySqlDataAccess(_connectionString);
            var estado = message.Sent ? 1 : 2;
            return dataAccess.ExecuteText(sql, new
            {
                message.FirstName,
                message.LastName,
                message.ToAddress,
                message.CcAddress,
                message.Subject,
                message.Title,
                message.Body,
                Estado = estado
            }) > 0;
        }
    }
}
