using MailKit.Net.Smtp;
using MimeKit;
using User.Management.Service.Models;

namespace User.Management.Service.Services
{
	public class EmailService : IEmailServices
	{
        private EmailConfiguration _emailConfig;

        public EmailService(EmailConfiguration emailConfiguration) => _emailConfig = emailConfiguration;
		
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = message.Content
            };

            return emailMessage;
        }

        private void Send(MimeMessage mailmessage)
        {
            using var client = new SmtpClient();

            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.Username, _emailConfig.Password);

                client.Send(mailmessage);
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }


    }
}

