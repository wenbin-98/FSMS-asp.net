using FSMS_asp.net.Data;
using FSMS_asp.net.Models;
using FSMS_asp.net.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FSMS_asp.net.Controllers
{
    public class UsersController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(IAccountRepository accountRepository, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _accountRepository = accountRepository;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            //get all users list
            var UsersList = await _context.Users.ToListAsync();

            List<UsersIndexViewModel> Users = new List<UsersIndexViewModel>();

            foreach (var item in UsersList)
            {
                UsersIndexViewModel user = new UsersIndexViewModel();
                user.Address = item.Address;
                user.PhoneNumber = item.PhoneNumber;
                user.Name = item.Name;
                user.DateOfBirth = item.DateOfBirth;
                user.Id = item.Id;
                var role = await _userManager.GetRolesAsync(item);
                user.Role = role[0];

                Users.Add(user);
            }

            return _context.CustomersModel != null ?
                          View(Users) :
                          Problem("Entity set 'ApplicationDbContext.CustomersModel'  is null.");
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.CreateUserAsync(model);
                if (!result.Succeeded)
                {
                    foreach (var errorMessage in result.Errors)
                    {
                        ModelState.AddModelError("", errorMessage.Description);
                    }
                    return View(model);
                }

                var NewUser = await _userManager.FindByEmailAsync(model.Email);

                await _userManager.AddToRoleAsync(NewUser, "Staff");

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(string? Id)

        {
            if (Id == null || _context.Users == null)
            {
                return NotFound();
            }

            var Users = await _userManager.Users.ToListAsync();
            var User = Users.Find(x => x.Id == Id);
            if (User == null)
            {
                return NotFound();
            }
            return View(User);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(id);
                    user.PhoneNumber = model.PhoneNumber;
                    user.Address = model.Address;
                    user.Name = model.Name;
                    user.DateOfBirth = model.DateOfBirth;
                    await _userManager.UpdateAsync(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_userManager.Users == null)
            {
                return Problem("Entity set 'dbo.AspNetUsers' is null.");
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.RemoveFromRoleAsync(user, "Staff");
            await _userManager.AddToRoleAsync(user, "Manager");

            return Redirect("/users/index");
        }
    }
}
