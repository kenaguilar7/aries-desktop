using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Email;

namespace AriesContador.Core.Services
{
    public interface IEmailService
    {
        Task<DataTable> GetLogAsync(CancellationToken cancellationToken = default);
        Task<bool> InsertAsync(MailMessageLog message, CancellationToken cancellationToken = default);
        Task<IEnumerable<MailMessageLog>> SendMailAsync(IEnumerable<MailMessageLog> messages, CancellationToken cancellationToken = default);
    }
}
