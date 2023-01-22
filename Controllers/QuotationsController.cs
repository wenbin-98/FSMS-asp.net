using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FSMS_asp.net.Data;
using FSMS_asp.net.Models.Quotation;
using Newtonsoft.Json;

namespace FSMS_asp.net.Controllers
{
    public class QuotationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuotationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Quotations
        public async Task<IActionResult> Index()
        {
            //get all data of quotation
            var model = await _context.QuotationsModel.ToListAsync();

            //return view with data of quotation
            return _context.QuotationsModel != null ?
                        View(model) :
                        Problem("Entity set 'ApplicationDbContext.InvoicesModel'  is null.");
        }

        // GET: Quotations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //find the specific quotation by using id
            var quotationsModel = await _context.QuotationsModel.Where(x => x.Id == id).FirstOrDefaultAsync();

            //if specific quotation not found then return not found page
            if (quotationsModel == null)
            {
                return NotFound();
            }

            //include all quotation data in a model
            var quotationsViewModel = new QuotationsViewModel
            {
                Id = quotationsModel.Id,
                CustomersId = quotationsModel.CustomersId,
                CustomerAddress = quotationsModel.CustomerAddress,
                CustomerEmail = quotationsModel.CustomerEmail,
                CustomerName = quotationsModel.CustomerName,
                CustomerHpNo = quotationsModel.CustomerHpNo,
                Date = quotationsModel.Date,
                TotalAmount = quotationsModel.TotalAmount,
                Products = await _context.ProductsModel.ToListAsync(),
                QuotationDetails = await _context.QuotationDetailsModel.Where(x => x.QuotationId == id).ToListAsync(),
                CancelStatus = quotationsModel.CancelStatus,
            };

            //return quotation data with quotation detail page
            return View(quotationsViewModel);
        }

        // GET: Quotations/Create
        public async Task<IActionResult> Create()
        {
            //get all customer name (for dropdown in quotation)
            ViewBag.Customers = _context.CustomersModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            //get all product name (for dropdown in quotation)
            ViewBag.Products = _context.ProductsModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            int NewQuotationId = 0;
            //set the id of quotation (use to show the current quotation id in the page)
            try
            {
                NewQuotationId = await _context.QuotationsModel.MaxAsync(x => x.Id) + 1;
                ViewBag.NewQuotationId = NewQuotationId;
            }
            catch
            {
                ViewBag.NewQuotationId = 0;
            }

            //include all products data in the model
            QuotationsViewModel quotationsViewModel = new QuotationsViewModel
            {
                Products = await _context.ProductsModel.ToListAsync()
            };

            //return all products and customer data to the create quotation view 
            return View(quotationsViewModel);
        }

        [HttpPost]
        public async Task AddQuotationDetail([FromBody] List<QuotationDetailsPost> QuotationDetails)
        {
            try
            {
                var ProductsData = await _context.ProductsModel.ToListAsync();
                var NewQuotationNumber = await _context.QuotationsModel.MaxAsync(x => x.Id);

                foreach (var QuotationDetail in QuotationDetails)
                {
                    QuotationDetailsModel newDetails = new QuotationDetailsModel
                    {
                        ProductId = QuotationDetail.Id,
                        QuotationId = NewQuotationNumber,
                        ProductName = ProductsData.Find(x => x.Id == QuotationDetail.Id).Name,
                        ProductPrice = ProductsData.Find(x => x.Id == QuotationDetail.Id).Price,
                        Quantity = QuotationDetail.Quantity,
                    };
                    await _context.QuotationDetailsModel.AddAsync(newDetails);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                ;
            }
        }

        public async Task RemoveQuotationDetail(int quotationId)
        {
            var quotationDetails = await _context.QuotationDetailsModel.Where(x => x.QuotationId == quotationId).ToListAsync();
            foreach (var quotationDetail in quotationDetails)
            {
                _context.QuotationDetailsModel.Remove(quotationDetail);
            }
            await _context.SaveChangesAsync();
        }

        // POST: Quotations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuotationsViewModel model)
        {
            //if model state is valid
            if (ModelState.IsValid)
            {
                //get the data of the customer chosen in the quotation
                var Customer = _context.CustomersModel.Where(x => x.Id == model.CustomersId).FirstOrDefault();

                //assign the information of quotation into a model
                QuotationsModel Quotation = new QuotationsModel
                {
                    Id = model.Id,
                    CustomersId = model.CustomersId,
                    CustomerAddress = Customer.Address,
                    CustomerEmail = Customer.Email,
                    CustomerName = Customer.Name,
                    CustomerHpNo = Customer.HpNo,
                    Date = model.Date,
                    TotalAmount = model.TotalAmount,
                    CancelStatus = false
                };
                //add all data of quotation into the database
                _context.Add(Quotation);
                await _context.SaveChangesAsync();
                //add all quotation detail into database
                await AddQuotationDetail(JsonConvert.DeserializeObject<List<QuotationDetailsPost>>(model.QuotationDetailsJson));
                //redirect back to quotation index page
                return RedirectToAction(nameof(Index));
            }
            //return submitted data to create invoice page
            return View(model);
        }

        // GET: Quotations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //find specific quotation by using id
            var quotationsModel = await _context.QuotationsModel.FindAsync(id);
            //if quotation not found then return quotation not found page
            if (quotationsModel == null)
            {
                return NotFound();
            }

            //get all customer name (for dropdown in quotation)
            ViewBag.Customers = _context.CustomersModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            //get all products name (for dropdown in quotation)
            ViewBag.Products = _context.ProductsModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            //include all data of quotation in a model
            var quotationsViewModel = new QuotationsViewModel
            {
                Id = quotationsModel.Id,
                CustomersId = quotationsModel.CustomersId,
                CustomerAddress = quotationsModel.CustomerAddress,
                CustomerEmail = quotationsModel.CustomerEmail,
                CustomerName = quotationsModel.CustomerName,
                CustomerHpNo = quotationsModel.CustomerHpNo,
                Date = quotationsModel.Date,
                TotalAmount = quotationsModel.TotalAmount,
                Products = await _context.ProductsModel.ToListAsync(),
                QuotationDetails = await _context.QuotationDetailsModel.Where(x => x.QuotationId == id).ToListAsync()
            };
            //set the id of the quotation
            int NewQuotationId = 0;

            try
            {
                NewQuotationId = await _context.QuotationsModel.MaxAsync(x => x.Id) + 1;
                ViewBag.NewQuotationId = NewQuotationId;
            }
            catch
            {
                ViewBag.NewQuotationId = 0;
            }
            //return view with data of quotation
            return View(quotationsViewModel);
        }

        // POST: Quotations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuotationsViewModel model)
        {
            //if model state is valid
            if (ModelState.IsValid)
            {
                //get the customer of the quotation
                var Customer = _context.CustomersModel.Where(x => x.Id == model.CustomersId).FirstOrDefault();

                //put all information into a model
                QuotationsModel Quotation = new QuotationsModel
                {
                    Id = model.Id,
                    CustomersId = model.CustomersId,
                    CustomerAddress = Customer.Address,
                    CustomerEmail = Customer.Email,
                    CustomerName = Customer.Name,
                    CustomerHpNo = Customer.HpNo,
                    Date = model.Date,
                    TotalAmount = model.TotalAmount,
                    CancelStatus = false
                };

                //update the quotation
                _context.Update(Quotation);
                await _context.SaveChangesAsync();
                //remove all quotation detail
                await RemoveQuotationDetail(id);
                try
                {
                    //add all quotation detail into database
                    await AddQuotationDetail(JsonConvert.DeserializeObject<List<QuotationDetailsPost>>(model.QuotationDetailsJson));
                }
                catch
                {

                }
                //redirect to quotation index page
                return RedirectToAction(nameof(Index));
            }
            //return all submitted data to edit quotation page
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int? id)
        {
            //find the specific quotation by using id
            var quotationsModel = await _context.QuotationsModel
                .FirstOrDefaultAsync(m => m.Id == id);
            //if quotation not found then return quotation not found page
            if (quotationsModel == null)
            {
                return NotFound();
            }

            //set quotation cancel status to true
            quotationsModel.CancelStatus = true;
            //update the specific quotation
            _context.QuotationsModel.Update(quotationsModel);
            await _context.SaveChangesAsync();

            //redirect to quotation index page
            return Redirect("/quotations/index"); ;
        }
    }
}
