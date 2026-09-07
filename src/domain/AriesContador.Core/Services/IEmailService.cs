using System.Collections.Generic;
using System.Data;
using AriesContador.Core.Models.Email;

namespace AriesContador.Core.Services
{
    public interface IEmailService
    {
        DataTable GetLog();
        bool Insert(MailMessageLog message);
        IEnumerable<MailMessageLog> SendMail(IEnumerable<MailMessageLog> messages);
    }
}
