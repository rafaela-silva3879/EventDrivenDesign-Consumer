using ConsumerPedidos.Settings;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace ConsumerPedidos.Messages
{
    public class PedidosMessage
    {
        private readonly MailSettings? _mailSettings;
        public PedidosMessage(IOptions<MailSettings>? mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public void Send(string to, string subject, string body)
        {
            var mailMessage = new MailMessage(_mailSettings.Account, to);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;
            var smtpClient = new SmtpClient(_mailSettings.Smtp, _mailSettings.Port.Value);
            smtpClient.Credentials = new NetworkCredential
                                        (_mailSettings.Account, _mailSettings.Password);
            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);
        }
    }
}