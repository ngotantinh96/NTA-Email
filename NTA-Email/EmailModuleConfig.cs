namespace NTA_Email
{
    internal partial class EmailClientService
    {
        public class EmailModuleConfig
        {
            public MailClient Imap { get; set; }
            public MailClient Smtp { get; set; }
        }

        public class MailClient
        {
            public string Host { get; set; }
            public string Port { get; set; }
            public string User { get; set; }
            public string Password { get; set; }
        }
    }
}
