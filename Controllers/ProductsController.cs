using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FSMS_asp.net.Data;
using Microsoft.AspNetCore.Hosting;
using FSMS_asp.net.Models;

namespace FSMS_asp.net.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
              return _context.ProductsModel != null ? 
                          View(await _context.ProductsModel.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.ProductsModel'  is null.");
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductsModel == null)
            {
                return NotFound();
            }

            var productsModel = await _context.ProductsModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productsModel == null)
            {
                return NotFound();
            }

            return View(productsModel);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductsViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);
                ProductsModel products = new ProductsModel
                {
                    Name = model.Name,
                    Price = model.Price,
                    Quantity = model.Quantity,
                    Description = model.Description,
                    UpdatedAt = DateTime.Now,
                    Image = uniqueFileName
                };

                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductsModel == null)
            {
                return NotFound();
            }

            var productsModel = await _context.ProductsModel.FindAsync(id);
            if (productsModel == null)
            {
                return NotFound();
            }
            return View(productsModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductsViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.ProductsModel.FindAsync(model.Id);
                    product.Name = model.Name;
                    product.Price = model.Price;
                    product.Quantity = model.Quantity;
                    product.Description = model.Description;
                    product.UpdatedAt = DateTime.Now;

                    if (product.Image != null)
                    {
                        if (model.ExistingImage != null)
                        {
                            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/Products Image", model.ExistingImage);
                            System.IO.File.Delete(filePath);
                        }
                        product.Image = ProcessUploadedFile(model);
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsModelExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductsModel == null)
            {
                return NotFound();
            }

            var productsModel = await _context.ProductsModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productsModel == null)
            {
                return NotFound();
            }

            return View(productsModel);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductsModel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProductsModel'  is null.");
            }
            var productsModel = await _context.ProductsModel.FindAsync(id);
            if (productsModel != null)
            {
                try
                {
                    string imagePath = productsModel.Image.Remove(0, 1);
                    imagePath = imagePath.Replace('/', '\\');
                    var CurrentImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\", imagePath);
                    CurrentImage = CurrentImage.Replace('\\', '/');
                    _context.ProductsModel.Remove(productsModel);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        if (System.IO.File.Exists(CurrentImage))
                        {
                            System.IO.File.Delete(CurrentImage);
                        }
                    }
                } 
                catch 
                {
                    _context.ProductsModel.Remove(productsModel);
                    await _context.SaveChangesAsync();
                }
                
            }
            
            
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsModelExists(int id)
        {
          return (_context.ProductsModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string ProcessUploadedFile(ProductsViewModel model)
        {
            string uniqueFileName = null;

            if (model.Image != null)
            {
                string folder = "images/Products Image/";
                folder += Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                uniqueFileName = "/" + folder;

                //string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "/images/Products Image");
                //uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                //string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(serverFolder, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
