namespace AriesContador.Core.Models.Email
{
    public class MailMessageLog
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ToAddress { get; set; }
        public string CcAddress { get; set; }
        public string Subject { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Sent { get; set; }
    }

    public class SmtpOptions
    {
        public string Host { get; set; }
        public int Port { get; set; } = 587;
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FromAddress { get; set; }
        public string FromDisplayName { get; set; } = "Sistemas Aries";
    }
}
