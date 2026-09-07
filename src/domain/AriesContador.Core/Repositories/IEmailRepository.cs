using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Email;

namespace AriesContador.Core.Repositories
{
    public interface IEmailRepository
    {
        Task<DataTable> GetLogAsync(CancellationToken cancellationToken = default);
        Task<bool> InsertAsync(MailMessageLog message, CancellationToken cancellationToken = default);
    }
}
