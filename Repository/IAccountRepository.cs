using FSMS_asp.net.Models;
using FSMS_asp.net.Models.Account;
using FSMS_asp.net.Models.Login;
using Microsoft.AspNetCore.Identity;

namespace FSMS_asp.net.Repository
{
    public interface IAccountRepository
    {
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordModel model);
        Task<IdentityResult> ConfirmEmailAsync(string uid, string token);
        Task<IdentityResult> CreateUserAsync(RegisterModel userModel);
        Task GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task GenerateForgotPasswordTokenAsync(ApplicationUser user);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<SignInResult> PasswordSignInAsync(LoginModel loginModel);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model);
        Task SignOutAsync();
    }
}