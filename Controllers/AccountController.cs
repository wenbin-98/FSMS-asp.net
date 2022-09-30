using FSMS_asp.net.Models;
using FSMS_asp.net.Models.Account;
using FSMS_asp.net.Repository;
using FSMS_asp.net.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FSMS_asp.net.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public AccountController(SignInManager<ApplicationUser> signInManager, IAccountRepository accountRepository, UserManager<ApplicationUser> userManager, IUserService userService)
        {
            _signInManager = signInManager;
            _accountRepository = accountRepository;
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userService.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            else
            {

                AccountIndexViewModel model = new AccountIndexViewModel()
                {
                    ChangePasswordModel = new ChangePasswordModel(),
                    EditInformationModel = new EditInformationModel()
                    {
                        Name = user.Name,
                        DateOfBirth = user.DateOfBirth,
                        Address = user.Address,
                        PhoneNumber = user.PhoneNumber
                    }
                };

                return View("~/Views/Account/Index.cshtml", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var userId = _userService.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);

            AccountIndexViewModel accountIndexViewModel = new AccountIndexViewModel()
            {
                ChangePasswordModel = new ChangePasswordModel(),
                EditInformationModel = new EditInformationModel()
                {
                    Name = user.Name,
                    DateOfBirth = user.DateOfBirth,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber
                }
            };

            if (ModelState.IsValid)
            {
                var result = await _accountRepository.ChangePasswordAsync(model);
                if (result.Succeeded)
                {
                    ViewBag.IsSuccess = true;
                    ModelState.Clear();

                    
                    return View("~/Views/Account/Index.cshtml", accountIndexViewModel);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View("~/Views/Account/Index.cshtml", accountIndexViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditPersonalInformation(EditInformationModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userService.GetUserId();
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    user.Name = model.Name;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Address = model.Address;
                    user.DateOfBirth = model.DateOfBirth;

                    await _userManager.UpdateAsync(user);
                }

                AccountIndexViewModel accountIndexViewModel = new AccountIndexViewModel()
                {
                    ChangePasswordModel = new ChangePasswordModel(),
                    EditInformationModel = model,
                };

                ViewBag.IsSuccess = true;

                return View("~/Views/Account/Index.cshtml", accountIndexViewModel);
            }

            return View("~/Views/Account/Index.cshtml");
        }
    }
}
