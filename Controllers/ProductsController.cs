using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FSMS_asp.net.Data;
using FSMS_asp.net.Models;
using System.Security.Claims;

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
            //return view with all products data
              return _context.ProductsModel != null ? 
                          View(await _context.ProductsModel.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.ProductsModel'  is null.");
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //find specific products by using id
            var productsModel = await _context.ProductsModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productsModel == null)
            {
                return NotFound();
            }
            //return view with product's data
            return View(productsModel);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            //return create view
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductsViewModel model)
        {
            //if model state is valid
            if (ModelState.IsValid)
            {
                //upload the image of products and get the uniqied file name of image
                string uniqueFileName = ProcessUploadedFile(model);
                //set the data of product in a model
                ProductsModel products = new ProductsModel
                {
                    Name = model.Name,
                    Price = model.Price,
                    Quantity = model.Quantity,
                    Description = model.Description,
                    UpdatedAt = DateTime.Now,
                    Image = uniqueFileName,
                    UpdatedBy = User.FindFirstValue(ClaimTypes.Name)
                };

                //add the products into database
                _context.Add(products);
                await _context.SaveChangesAsync();
                //redirect to products index page
                return RedirectToAction(nameof(Index));
            }
            //return view with submitted model
            return View(model);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //find specific product in database by using id
            var productsModel = await _context.ProductsModel.FindAsync(id);
            //if found no product then return not found page
            if (productsModel == null)
            {
                return NotFound();
            }
            //return view with product data
            return View(productsModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductsViewModel model)
        {
            //if model state is valid
            if (ModelState.IsValid)
            {
                //try to update specific product with submitted data
                try
                {
                    var product = await _context.ProductsModel.FindAsync(model.Id);
                    product.Name = model.Name;
                    product.Price = model.Price;
                    product.Quantity = model.Quantity;
                    product.Description = model.Description;
                    product.UpdatedAt = DateTime.Now;
                    product.UpdatedBy = User.FindFirstValue(ClaimTypes.Name);
                    //if product image is not null
                    if (model.Image != null )
                    {
                        //if product image exist in storage, then delete the image
                        if (product.Image != null)
                        {
                            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, product.Image);
                            System.IO.File.Delete(filePath);
                        }
                        //save the image
                        product.Image = ProcessUploadedFile(model);
                    }
                    //update the information of product in database
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
                //redirect to product index page
                return RedirectToAction(nameof(Index));
            }
            //return edit view with submitted data
            return View(model);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //find specific product by using id
            var productsModel = await _context.ProductsModel.FindAsync(id);
            if (productsModel != null)
            {
                //try to delete the image of products if image of products exists
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
                    //remove the product from database
                    _context.ProductsModel.Remove(productsModel);
                    await _context.SaveChangesAsync();
                }
                
            }
            
            //redirect to product index page
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsModelExists(int id)
        {
          return (_context.ProductsModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string ProcessUploadedFile(ProductsViewModel model)
        {
            string uniqueFileName = null;

            //if image of product is uploaded
            if (model.Image != null)
            {
                //create a unique file name for product image
                string folder = "images/Products Image/";
                folder += Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                uniqueFileName = "/" + folder;

                //put the image in the storage
                using (var fileStream = new FileStream(serverFolder, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }
            //return the file name of image
            return uniqueFileName;
        }
    }
}
