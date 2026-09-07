using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Mail;
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

        public DataTable GetLog()
        {
            return _unitOfWork.EmailRepository.GetLog();
        }

        public bool Insert(MailMessageLog message)
        {
            return _unitOfWork.EmailRepository.Insert(message);
        }

        public IEnumerable<MailMessageLog> SendMail(IEnumerable<MailMessageLog> messages)
        {
            var rejected = new List<MailMessageLog>();
            foreach (var user in messages)
            {
                user.Sent = Push(user);
                if (!user.Sent)
                    rejected.Add(user);
                Insert(user);
            }
            return rejected;
        }

        private bool Push(MailMessageLog usuario)
        {
            if (string.IsNullOrWhiteSpace(_smtp.Host) || string.IsNullOrWhiteSpace(_smtp.UserName))
                return false;

            try
            {
                var client = new SmtpClient(_smtp.Host, _smtp.Port)
                {
                    Credentials = new NetworkCredential(_smtp.UserName, _smtp.Password ?? string.Empty),
                    EnableSsl = true
                };
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
                client.Send(mail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
