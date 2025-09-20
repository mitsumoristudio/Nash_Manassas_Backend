namespace Project_Manassas.Service;

public interface ISendGridService
{
    Task<bool> SendEmailAsync(string email, string subject, string htmlContent);
}