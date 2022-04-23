# NTA-Email
A. Third-party services/technologies/libraries in use for NTA Email module:
  1. Mailkit: https://github.com/jstedfast/MailKit
    1.1 Mail Protocals in use:
      1.1.1 IMAP: https://en.wikipedia.org/wiki/Internet_Message_Access_Protocol
      1.1.2 SMTP: https://en.wikipedia.org/wiki/Simple_Mail_Transfer_Protocol
      1.1.3 How to enable in IMAP/SMTP servers Gmail: https://support.google.com/mail/answer/7126229?hl=vi#zippy=%2Cb%C6%B0%E1%BB%9Bc-ki%E1%BB%83m-tra-%C4%91%E1%BB%83-%C4%91%E1%BA%A3m-b%E1%BA%A3o-r%E1%BA%B1ng-b%E1%BA%A1n-%C4%91%C3%A3-b%E1%BA%ADt-imap%2Cb%C6%B0%E1%BB%9Bc-thay-%C4%91%E1%BB%95i-smtp-v%C3%A0-c%C3%A1c-ch%E1%BA%BF-%C4%91%E1%BB%99-c%C3%A0i-%C4%91%E1%BA%B7t-kh%C3%A1c-trong-ch%C6%B0%C6%A1ng-tr%C3%ACnh-email-kh%C3%A1ch
    1.2 Mailkit methods/class/inteface/property/etc in use:
      1.2.1: Connect(): Establish a connection to the specified IMAP server
      1.2.2: Authenticate(): Authenticate using the specified user name and password.
      1.2.3: Disconnect(): Disconnect the service.
      1.2.4: Dispose(): Releases the unmanaged resources used by the MailKit.MailService and optionally releases the managed resources.
      1.2.5: Inbox.Open(): opens the inbox folder using the requested folder access.
      1.2.6: SearchQuery: A specialized query for searching messages in a MailKit.IMailFolder.
      1.2.7: UniqueId: A unique identifier for email
      1.2.8: IMessageSummary: A summary of a message.
      1.2.9: HeaderId: An enumeration of common header fields.
      1.2.10: FolderAccess:  A folder access mode.
      1.2.11: Inbox.Fetch(): Fetch the message summaries for the specified message UIDs.
      1.2.12: Inbox.Search(): Search the folder for messages matching the specified query.
      1.2.13: MessageSummaryItems: A bitfield of MailKit.MessageSummary fields.
      1.2.14: MimeMessage: A MIME message.
      1.2.15: MimeEntity: An abstract MIME entity.
      1.2.16: MessagePart: A MIME part containing a MimeKit.MimeMessage as its content.
      1.2.17: MimePart: A leaf-node MIME part that contains content such as the message body text or an attachment.
      1.2.18: SendAsync(): Asynchronously send the specified message.
      
B. Enities/Tables for NTA Email module:
  1. Email_Settings: Store email account to filter mail from Gmail and settings for this email account
  2. Email_Folders: Store root folders, custom folder and custom subfolders
  3. Email_ThreadMessages: Store thread infor of multiple email messages
  4. Email_FolderMessages: Store N to N relationship between Folder and Thread mail
  5. Email_Messages: Store sent messages and messages that fetched from Gmail
  6. Email_PersonInCharges: Store person in charge from a thread email
  7. Email_Signatures: Store signatures settings for an email account
  8. Email_Spamfilters: Store spam filter rules for an email account

C. Terms and Definitions:
  1. Thread: group of related emails by subject.
  2. Attachments: mail attached files that sent or received in mail body
  3. Signature: signature of an users used when send/reply/reply all/ forward an email and set to the end of mail body
