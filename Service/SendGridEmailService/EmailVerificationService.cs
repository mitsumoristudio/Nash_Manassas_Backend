using Microsoft.EntityFrameworkCore;
using Project_Manassas.Database;
using Project_Manassas.Model;

namespace Project_Manassas.Service;

public class EmailVerificationService : IEmailVerificationService
{
    private readonly ProjectContext _dbContext;
    private readonly IEmailSenderService _emailSenderService;

    public EmailVerificationService(ProjectContext dbContext, IEmailSenderService emailSenderService)
    {
        _dbContext = dbContext;
        _emailSenderService = emailSenderService;
    }
    
    public async Task SendVerificationCode(string email)
    {
        // Invalidate old codes
        var oldCodes = _dbContext.VerificationCodes.Where(x => x.Email == email && !x.Used);
        _dbContext.VerificationCodes.RemoveRange(oldCodes);
        
        // Generate 6 point random number
        var random6Number = new Random().Next(100000, 999999).ToString();

        var newEntity = new VerificationCode
        {
            Email = email,
            Code = random6Number,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };

        await _dbContext.VerificationCodes.AddAsync(newEntity);
        await _dbContext.SaveChangesAsync();
        
       //    var verifyUrl = $"https://morisolution.org/verifyEmail?token={random6Number}";
         var verifyUrl = $"http://localhost:3000/verifyEmail";       
        
        var subject = "Your Verification Code";
        var body = $"Your verification code is: {random6Number}\n\nIt expires in 10 minutes." +
                         $"Click the link to verify your email: <a href='{verifyUrl}'>Verify</a>";
     
        await _emailSenderService.SendEmailAsync(email, subject, body);
    }

    public async Task<bool> VerifyCodeAsync(string email, string code)
    {
        var records = await _dbContext.VerificationCodes
            .Where(x => x.Email == email && !x.Used)
            .OrderByDescending(x => x.ExpiresAt)
            .FirstOrDefaultAsync();
        
        if (records == null) return false;
        
        if (records.ExpiresAt < DateTime.UtcNow) return false;

        records.AttemptCount++;
        if (records.AttemptCount > 5) return false;

        if (records.Code != code)
        {
            await _dbContext.SaveChangesAsync();
            return false;
        }
        
        records.Used = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}