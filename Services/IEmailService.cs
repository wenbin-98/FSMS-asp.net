using FSMS_asp.net.Models;

namespace FSMS_asp.net.Services
{
    public interface IEmailService
    {
        Task SendEmailForConfirmation(UserEmailOptions userEmailOptions);
        Task SendEmailForForgotPassword(UserEmailOptions userEmailOptions);
        Task SendTestEmail(UserEmailOptions userEmailOptions);
    }
}