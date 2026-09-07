using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core;
using AriesContador.Core.Models.Email;
using AriesContador.Core.Services;

namespace AriesContador.Services
{
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SmtpOptions _smtp;

        public EmailService(IUnitOfWork unitOfWork, SmtpOptions smtp)
        {
            _unitOfWork = unitOfWork;
            _smtp = smtp ?? new SmtpOptions();
        }

        public Task<DataTable> GetLogAsync(CancellationToken cancellationToken = default)
        {
            return _unitOfWork.EmailRepository.GetLogAsync(cancellationToken);
        }

        public Task<bool> InsertAsync(MailMessageLog message, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.EmailRepository.InsertAsync(message, cancellationToken);
        }

        public async Task<IEnumerable<MailMessageLog>> SendMailAsync(IEnumerable<MailMessageLog> messages, CancellationToken cancellationToken = default)
        {
            var rejected = new List<MailMessageLog>();
            foreach (var user in messages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                user.Sent = await PushAsync(user, cancellationToken).ConfigureAwait(false);
                if (!user.Sent)
                    rejected.Add(user);
                await InsertAsync(user, cancellationToken).ConfigureAwait(false);
            }
            return rejected;
        }

        private async Task<bool> PushAsync(MailMessageLog usuario, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_smtp.Host) || string.IsNullOrWhiteSpace(_smtp.UserName))
                return false;

            try
            {
                using (var client = new SmtpClient(_smtp.Host, _smtp.Port)
                {
                    Credentials = new NetworkCredential(_smtp.UserName, _smtp.Password ?? string.Empty),
                    EnableSsl = true
                })
                {
                    var mail = new MailMessage();
                    mail.From = new MailAddress(
                        string.IsNullOrWhiteSpace(_smtp.FromAddress) ? _smtp.UserName : _smtp.FromAddress,
                        _smtp.FromDisplayName ?? "Sistemas Aries");
                    mail.To.Add(new MailAddress(usuario.ToAddress));
                    if (!string.IsNullOrWhiteSpace(usuario.CcAddress))
                        mail.CC.Add(new MailAddress(usuario.CcAddress));
                    mail.Subject = usuario.Subject;
                    mail.IsBodyHtml = true;
                    mail.Body = $"<html><nav><h1>{usuario.Title}</h1><span>{usuario.Body}</span><h6></h6></nav></html>";
                    await client.SendMailAsync(mail).ConfigureAwait(false);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
