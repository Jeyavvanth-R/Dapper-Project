using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using System.Text.RegularExpressions;

namespace TestSPInsertFinal.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IConfiguration config;
        private readonly ILogger<EmailRepository> logger;

        public EmailRepository(IConfiguration config, ILogger<EmailRepository> logger)
        {
            this.config = config;
            this.logger = logger;
        }

        public void sendPassword(string toAddress, string content)
        {
            try
            {
                string newContent = string.Concat(config.GetSection("SMTP")["EmailContent"], " \n ", content);
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(config.GetSection("SMTP")["EmailUsername"]));
                email.To.Add(MailboxAddress.Parse(toAddress));
                email.Subject = config.GetSection("SMTP")["EmailSubject"];
                email.Body = new TextPart(TextFormat.Text)
                {
                    Text = newContent
                };

                using var smtp = new SmtpClient();
                smtp.Connect(config.GetSection("SMTP")["EmailHost"], 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(config.GetSection("SMTP")["EmailUsername"], config.GetSection("SMTP")["EmailPassword"]);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                logger.LogError("Error at EmailRepository.sendPassword: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public bool IsValidEmail(string email)
        {
            const string emailRegexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(email, emailRegexPattern);
        }
    }
}
