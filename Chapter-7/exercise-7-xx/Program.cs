// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System;
using System.Net;
using System.Net.Mail;

class Program
{
    static void Main(string[] args)
    {
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress("julian.soh@hotmail.com");
        mailMessage.To.Add("julian.soh@hotmail.com");
        mailMessage.Subject = "Subject";
        mailMessage.Body = "This is test email";

        //https://mailtrap.io/blog/outlook-smtp/#:~:text=To%20send%20emails%20through%20Outlook%20SMTP%2C%20input%20the,jane.doe%40live.com%29%20Password%3A%20The%20App%20password%20you%20generated%20earlier
        SmtpClient smtpClient = new SmtpClient();
        smtpClient.Host = "smtp-mail.outlook.com";
        smtpClient.Port = 587;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential("julian.soh@hotmail.com", "Fluffy&jasmak1");
        smtpClient.EnableSsl = true;

        try
        {
            smtpClient.Send(mailMessage);
            Console.WriteLine("Email Sent Successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
