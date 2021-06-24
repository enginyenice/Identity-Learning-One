using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One.Helper
{
    public static class EmailConfirmation
    {

        public static void EmailConfirmationSendMail(string link, string email)
        {
            //"test@enginyenice.com", "Qsj^ZTP0yWE6_*$%"
            MailMessage mailMessage = new MailMessage();
            SmtpClient smtpClient = new SmtpClient("mail.enginyenice.com");

            mailMessage.From = new MailAddress("test@enginyenice.com");
            mailMessage.To.Add(email);
            mailMessage.Subject = $"xxx.xxx Email Doğrulama";
            mailMessage.Body = "<h2>Hesabınızı doğrulamak için lütfen aşağıdaki linke tıklayınız</h2><hr/>";
            mailMessage.Body += $"<a href='{link}'>Doğrulama Linki</a>";
            mailMessage.IsBodyHtml = true;

            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential("test@enginyenice.com", "Qsj^ZTP0yWE6_*$%");
            smtpClient.Send(mailMessage);





        }
    }
}
