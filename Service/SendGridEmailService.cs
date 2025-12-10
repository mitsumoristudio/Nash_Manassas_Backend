namespace Project_Manassas.Service;

public class SendGridEmailService: IEmailSenderService
{
    private readonly string _apikey;

    public SendGridEmailService(IConfiguration config)
    {
        _apikey = config["SendGrid:ApiKey"];
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var client = new SendGrid.SendGridClient(_apikey);
        var message = new SendGrid.Helpers.Mail.SendGridMessage()
        {
            From = new SendGrid.Helpers.Mail.EmailAddress("no-reply@morisolution.com", "Mori Solution"),
            Subject = subject,
            HtmlContent = body,
        };
        message.AddTo(new SendGrid.Helpers.Mail.EmailAddress(to));
        
        await client.SendEmailAsync(message);
    }
}