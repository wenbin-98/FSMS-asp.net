using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FSMS_asp.net.Data;
using FSMS_asp.net.Models;

namespace FSMS_asp.net.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            //return view with all customers data
              return _context.CustomersModel != null ? 
                          View(await _context.CustomersModel.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.CustomersModel'  is null.");
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CustomersModel == null)
            {
                return NotFound();
            }
            //find customer by id
            var customersModel = await _context.CustomersModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customersModel == null)
            {
                return NotFound();
            }
            //return view with customer data
            return View(customersModel);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            //return create customer view
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,HpNo,Address,Email")] CustomersModel customersModel)
        {
            //if model state is valid
            if (ModelState.IsValid)
            {
                //add customer to database
                _context.Add(customersModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //return view with submitted data
            return View(customersModel);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CustomersModel == null)
            {
                return NotFound();
            }
            //find specific customers by id
            var customersModel = await _context.CustomersModel.FindAsync(id);
            if (customersModel == null)
            {
                return NotFound();
            }
            //return view with customer data
            return View(customersModel);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,HpNo,Address,Email")] CustomersModel customersModel)
        {
            if (id != customersModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //try to update customer with submitted data
                try
                {
                    _context.Update(customersModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomersModelExists(customersModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //redirect to customer index
                return RedirectToAction(nameof(Index));
            }
            //return edit view with submitted data
            return View(customersModel);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CustomersModel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CustomersModel'  is null.");
            }
            //find customer by id
            var customersModel = await _context.CustomersModel.FindAsync(id);
            //remove the customer from database
            if (customersModel != null)
            {
                _context.CustomersModel.Remove(customersModel);
            }
            
            await _context.SaveChangesAsync();
            //redirect to customer index page
            return RedirectToAction(nameof(Index));
        }

        private bool CustomersModelExists(int id)
        {
          return (_context.CustomersModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
