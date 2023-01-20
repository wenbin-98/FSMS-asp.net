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

            //list of users
            List<UsersIndexViewModel> Users = new List<UsersIndexViewModel>();

            //insert each user to a list
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

            //return view
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
        public async Task<IActionResult> Create(UsersCreateViewModel model)
        {
            //if model state is valid
            if (ModelState.IsValid)
            {
                //put all data in a user model
                var user = new ApplicationUser()
                {
                    Email = model.Email,
                    UserName = model.Email,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    Name = model.Name,
                };

                //create new user
                var result = await _userManager.CreateAsync(user, model.Password);
                //if not success
                if (!result.Succeeded)
                {
                    //return error message
                    foreach (var errorMessage in result.Errors)
                    {
                        ModelState.AddModelError("ModelOnly", errorMessage.Description);
                    }
                    return View(model);
                }

                var NewUser = await _userManager.FindByEmailAsync(model.Email);

                //set the user as staff
                await _userManager.AddToRoleAsync(NewUser, "Staff");

                //redirect to user index page
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

            //return view and data of the specific user
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

            //if model state is valid
            if (ModelState.IsValid)
            {
                //try change value of user 
                try
                {
                    var user = await _userManager.FindByIdAsync(id);
                    user.PhoneNumber = model.PhoneNumber;
                    user.Address = model.Address;
                    user.Name = model.Name;
                    user.DateOfBirth = model.DateOfBirth;
                    //update user
                    await _userManager.UpdateAsync(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }
                //redirect to user index
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
            //find the user by id
            var user = await _userManager.FindByIdAsync(id);
            //if user is found then delete the user
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            //return to user index
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            //get the user from database
            var user = await _userManager.FindByIdAsync(id);
            //remove the staff role from user
            await _userManager.RemoveFromRoleAsync(user, "Staff");
            //set admin to user
            await _userManager.AddToRoleAsync(user, "Manager");

            //redirect to user index
            return Redirect("/users/index");
        }
    }
}
