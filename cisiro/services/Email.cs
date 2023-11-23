using MimeKit;
using MailKit.Net.Smtp;

namespace cisiro.services;

public class Email
{
   public Email(string toEmail, string subject, string message, string fromEmail, string appKey)
   {
      SendEmailAsync(toEmail, subject, message);
   }
   
   static async Task SendEmailAsync(string toEmail, string subject, string message)
   {
      var emailMessage = new MimeMessage();
      emailMessage.From.Add(new MailboxAddress("", ""));
      emailMessage.To.Add(new MailboxAddress("", toEmail));
      emailMessage.Subject = subject;

      emailMessage.Body = new TextPart("plain")
      {
         Text = message
      };

      using var client = new SmtpClient();
      await client.ConnectAsync("smtp.gmail.com", 587, false);
      await client.AuthenticateAsync("", "");
      await client.SendAsync(emailMessage);
      await client.DisconnectAsync(true);
   }
}