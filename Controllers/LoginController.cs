using FSMS_asp.net.Models.Login;
using FSMS_asp.net.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FSMS_asp.net.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public LoginController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.PasswordSignInAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Invalid credentials.");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Account is blocked, Try after some time.");
                }

                ModelState.AddModelError("", "Account is not found.");

            }

            return View("~/Views/Login/Index.cshtml", model);
        }

        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountRepository.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }

        [AllowAnonymous, HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous, HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _accountRepository.GetUserByEmailAsync(model.Email);
                if (user != null)
                {
                    await _accountRepository.GenerateForgotPasswordTokenAsync(user);
                }

                ModelState.Clear();
                model.EmailSent = true;
            }
            return View("~/Views/Login/ForgotPassword.cshtml", model);
        }

        [AllowAnonymous, HttpGet("ResetPassword")]
        public IActionResult ResetPassword(string uid, string token)
        {
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel
            {
                Token = token,
                UserId = uid
            };
            return View(resetPasswordModel);
        }

        [AllowAnonymous, HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                model.Token = model.Token.Replace(' ', '+');
                var result = await _accountRepository.ResetPasswordAsync(model);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    model.IsSuccess = true;
                    return View(model);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
    }
}
