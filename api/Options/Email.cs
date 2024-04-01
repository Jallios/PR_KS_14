using System.Net.Mail;
using System.Net;

namespace api.Options
{
    public class Email
    {
        public static async Task SendConfirmationEmail(string email, string userId)
        {
            var registerPageUrl = $"https://192.168.3.14:7153/api/Auth/confirmEmail?email={email}&userId={userId}";

            var body = $@"
        <p>Пожалуйста, подтвердите вашу электронную почту, перейдя по ссылке:</p>
        <a href=""{registerPageUrl}"" style=""display: inline-block; background-color: #4CAF50; color: white; padding: 10px 20px; text-align: center; text-decoration: none; border-radius: 5px;"">Подтвердить email</a>
    ";

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("dumdleeslaser@gmail.com", "arsq fruj smuc igzu"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage("dumdleeslaser@gmail.com", email, "Подтвердите вашу электронную почту", body)
            {
                IsBodyHtml = true
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
