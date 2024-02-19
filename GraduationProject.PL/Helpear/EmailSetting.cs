using System.Net.Mail;
using System.Net;
using GraduationProject.API.DTOS;

namespace GraduationProject.API.Helpear
{
    public static class EmailSetting
    {
        public static void SendEmail(Email email)
        {
            var client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("ib1118200@gmail.com", "qalyivpiknyllruj");
            client.Send("ib1118200@gmail.com", email.To, email.Subject, email.Body);
        }
    }
}
