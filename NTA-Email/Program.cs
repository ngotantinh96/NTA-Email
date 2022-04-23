// See https://aka.ms/new-console-template for more information
using NTA_Email;
using static NTA_Email.EmailClientService;

Console.WriteLine("Hello, World!");
EmailModuleConfig mailConfig = new EmailModuleConfig
{
    // Gmail configs for IMAP and SMTP
};

// New instance or inject service
EmailClientService emailClientService = new EmailClientService(mailConfig);
emailClientService.FetchNewEmails("email to fetch", null);