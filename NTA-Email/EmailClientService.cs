using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using System.Dynamic;

namespace NTA_Email
{
    internal partial class EmailClientService
    {
        private readonly EmailModuleConfig _mailConfig;

        public EmailClientService(
            EmailModuleConfig mailConfig)
        {
            /// Inject _mailConfigs from appsettings/configs...
            _mailConfig = mailConfig;
        }

        /// <summary>
        /// Fetch new messages by NTA email and avoid to fetch existed emails in the database
        /// </summary>
        /// <param name="filterIds"></param>
        /// <param name="email"></param>
        /// <returns>Message summaries: size, headers, date, body structure</returns>
        public dynamic FetchNewEmails(string email, List<UniqueId>? filterIds)
        {
            try
            {
                ImapClient imapClient = InitImapConnection();
                imapClient.Inbox.Open(FolderAccess.ReadOnly);
                // Get all uniqueIds of mail from inbox
                SearchQuery searchQuery = (filterIds != null && filterIds.Any())
                ? SearchQuery.ToContains(email).And(SearchQuery.Not(SearchQuery.Uids(filterIds))) : SearchQuery.ToContains(email);

                List<UniqueId> uniqueIds = imapClient.Inbox.Search(searchQuery).ToList();
                if (uniqueIds != null && uniqueIds.Any())
                {
                    // Get message summaries
                    List<IMessageSummary> messageSummaries = imapClient.Inbox.Fetch(uniqueIds, MessageSummaryItems.Headers
                    | MessageSummaryItems.InternalDate | MessageSummaryItems.Size | MessageSummaryItems.BodyStructure).ToList();
                    var returnData = messageSummaries.Select(messageSummary =>
                        new
                        {
                            GmailId = messageSummary.UniqueId.ToString(),
                            Size = messageSummary.Size,
                            IssueDate = messageSummary.Date,
                            FromAddress = messageSummary.Headers[HeaderId.From],
                            ToAddress = messageSummary.Headers[HeaderId.To],
                            Subject = messageSummary.Headers[HeaderId.Subject],
                            Cc = messageSummary.Headers[HeaderId.Cc] ?? string.Empty,
                            Bcc = messageSummary.Headers[HeaderId.Bcc] ?? string.Empty,
                            Body = string.Empty,
                            Attachments = string.Join(",", messageSummary.BodyParts.Select(x => x.FileName).Where(x => x != null)),
                        });

                    imapClient.Disconnect(true);
                    imapClient.Dispose();
                    return returnData;
                }

                imapClient.Disconnect(true);
                imapClient.Dispose();
                return null;
            }
            catch (Exception e)
            {
                //Catch the error while working with Gmail Imap
                return null;
            }
        }



        /// <summary>
        /// Get mail body and attachments
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <returns>Body and Attachments</returns>
        public dynamic GetContentEmails(UniqueId uniqueId)
        {
            try
            {
                ImapClient imapClient = InitImapConnection();
                imapClient.Inbox.Open(FolderAccess.ReadOnly);
                MimeMessage inboxMessage = imapClient.Inbox.GetMessage(uniqueId);
                dynamic returnData = new ExpandoObject();
                if (inboxMessage != null)
                {
                    returnData.Body = inboxMessage.HtmlBody;
                    returnData.Attachments = new List<byte[]>();
                    foreach (IEnumerable<MimeEntity> attachment in inboxMessage.Attachments)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            if (attachment is MessagePart)
                            {
                                var part = (MessagePart)attachment;

                                part.Message.WriteTo(stream);
                            }
                            else
                            {
                                var part = (MimePart)attachment;

                                part.Content.DecodeTo(stream);
                            }
                            returnData.Attachments.Add(stream.ToArray());
                        }
                    }
                }
                imapClient.Disconnect(true);
                imapClient.Dispose();
                return returnData;
            }
            catch (Exception e)
            {
                return null;
                //Catch the error while working with Gmail Imap
            }
        }

        /// <summary>
        /// Send mail using SmtpClient with SSL
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Send mail result</returns>
        public async Task<bool> SendMail(MimeMessage email)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Connect(_mailConfig.Smtp.Host, int.Parse(_mailConfig.Smtp.Port), SecureSocketOptions.SslOnConnect);
                smtpClient.Authenticate(_mailConfig.Smtp.User, _mailConfig.Smtp.Password);
                await smtpClient.SendAsync(email);
                smtpClient.Disconnect(true);
                smtpClient.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permanently delete messages on Gmail
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <returns>Delete result status</returns>
        public bool DeleteMessages(IList<UniqueId> uniqueIds)
        {
            try
            {
                ImapClient imapClient = InitImapConnection();
                imapClient.Inbox.Open(FolderAccess.ReadOnly);
                imapClient.Inbox.AddFlags(uniqueIds, MessageFlags.Deleted, true);
                imapClient.Inbox.Expunge();
                imapClient.Disconnect(true);
                imapClient.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Open connection using ImapClient with SSL
        /// </summary>
        /// <returns>An IMAP client that can be used to retrieve messages from a server</returns>
        private ImapClient InitImapConnection()
        {
            ImapClient imapClient = new ImapClient
            {
                // Bypass check SSL certificate check for gmail server
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };
            // Connect and authenticate to gmail's imap using ssl connection
            imapClient.Connect(_mailConfig.Imap.Host, int.Parse(_mailConfig.Imap.Port), SecureSocketOptions.SslOnConnect);
            imapClient.Authenticate(_mailConfig.Imap.User, _mailConfig.Imap.Password);
            return imapClient;
        }
    }
}
