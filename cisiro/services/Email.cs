using SendGrid.Helpers.Mail;
using SendGrid;


namespace cisiro.services;

public class Email
{
    public  Email(string toEmail, string fromEmail, string subject, string mailContent, 
        string nameSender, string nameReceiver, string strKey)
        {
        Execute(toEmail, fromEmail, subject, mailContent, nameSender,
            nameReceiver, strKey).Wait();

        }

    static async Task Execute(string toEmail, string fromEmail, string subject, string mailContent,
        string nameSender, string nameReceiver, string strKey)
    {
        var apiKey = strKey;
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(fromEmail, nameSender);
        var to = new EmailAddress(toEmail, nameReceiver);
        var plainTextContent = mailContent;
        var htmlContent = "<strong>" + plainTextContent + "</strong>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);
    }

}