using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One.Helper
{
    public static class PasswordReset
    {
        public static void PasswordResetSendMail(string link)
        {
            //"test@enginyenice.com", "Qsj^ZTP0yWE6_*$%"
            MailMessage mailMessage = new MailMessage();
            SmtpClient smtpClient = new SmtpClient("mail.enginyenice.com");

            mailMessage.From = new MailAddress("test@enginyenice.com");
            mailMessage.To.Add("enginyenice2626@gmail.com");
            mailMessage.Subject = $"xxx.xxx Şifre Sıfırlama";
            mailMessage.Body = "<h2>Şifrenizi yenilemek için lütfen aşağıdaki linke tıklayınız</h2><hr/>";
            mailMessage.Body += $"<a href='{link}'>Şifre Yenileme Linki</a>";
            mailMessage.IsBodyHtml = true;

            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential("test@enginyenice.com", "Qsj^ZTP0yWE6_*$%");
            smtpClient.Send(mailMessage);





        }
    }
}
