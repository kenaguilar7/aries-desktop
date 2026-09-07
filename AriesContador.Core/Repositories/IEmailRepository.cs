using System.Data;
using AriesContador.Core.Models.Email;

namespace AriesContador.Core.Repositories
{
    public interface IEmailRepository
    {
        DataTable GetLog();
        bool Insert(MailMessageLog message);
    }
}
