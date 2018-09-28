using System.Net.Mail;

namespace SistemaTurnos.WebApplication.Email
{
    public class EmailSender
    {
        private static readonly string host = "localhost";
        private static readonly int port = 25;
        //private static readonly string username = "fernando";
        //private static readonly string password = "fernando";
        private static readonly MailAddress mailAddress = new MailAddress("no-reply@turnos.com.ar");

        public static void Send(EmailMessage email)
        {
            var smtpClient = new SmtpClient
            {
                Host = host,
                Port = port,
                //UseDefaultCredentials = false,
                // Credentials = new NetworkCredential(username, password)
            };

            MailMessage mailMessage = new MailMessage
            {
                From = mailAddress,
                Subject = email.Subject,
                Body = email.Message
            };

            foreach (var to in email.To)
            {
                mailMessage.To.Add(to);
            }

            smtpClient.Send(mailMessage);
        }
    }
}
