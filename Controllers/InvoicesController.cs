using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FSMS_asp.net.Data;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using FSMS_asp.net.Models;

namespace FSMS_asp.net.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            //get all data of invoice
            var model = await _context.InvoicesModel.ToListAsync();

            //return view with data of invoice
            return _context.InvoicesModel != null ?
                        View(model) :
                        Problem("Entity set 'ApplicationDbContext.InvoicesModel'  is null.");
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //find the specific invoice by using id
            var invoicesModel = await _context.InvoicesModel.Where(x => x.Id == id).FirstOrDefaultAsync();

            //if specific invoice not found then return not found page
            if (invoicesModel == null)
            {
                return NotFound();
            }

            //include all invoice data in a model
            var invoicesViewModel = new InvoicesViewModel
            {
                Id = invoicesModel.Id,
                CustomersId = invoicesModel.CustomersId,
                CustomerAddress = invoicesModel.CustomerAddress,
                CustomerEmail = invoicesModel.CustomerEmail,
                CustomerName = invoicesModel.CustomerName,
                CustomerHpNo = invoicesModel.CustomerHpNo,
                Date = invoicesModel.Date,
                PoNo = invoicesModel.PoNo,
                RefNo = invoicesModel.RefNo,
                TotalAmount = invoicesModel.TotalAmount,
                Products = await _context.ProductsModel.ToListAsync(),
                InvoiceDetails = await _context.InvoiceDetailsModel.Where(x => x.InvoiceId == id).ToListAsync(),
                CancelStatus = invoicesModel.CancelStatus,
            };

            //return invoice data with invoice detail page
            return View(invoicesViewModel);
        }

        // GET: Invoices/Create
        public async Task<IActionResult> Create()
        {
            //get all customer name (for dropdown in invoice)
            ViewBag.Customers = _context.CustomersModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            //get all product name (for dropdown in invoice)
            ViewBag.Products = _context.ProductsModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            int NewInvoiceId = 0;
            //set the id of invoice (use to show the current invoice id in the page)
            try
            {
                NewInvoiceId = await _context.InvoicesModel.MaxAsync(x => x.Id) + 1;
                ViewBag.NewInvoiceId = NewInvoiceId.ToString("0000000000"); ;
            }
            catch
            {
                ViewBag.NewInvoiceId = "0000000000";
            }

            //include all products data in the model
            InvoicesViewModel invoicesViewModel = new InvoicesViewModel
            {
                Products = await _context.ProductsModel.ToListAsync()
            };

            //return all products and customer data to the create invoice view 
            return View(invoicesViewModel);
        }

        [HttpPost]
        public async Task AddInvoiceDetail([FromBody] List<InvoiceDetailPost> InvoiceDetails)
        {
            try
            {
                var ProductsData = await _context.ProductsModel.ToListAsync();
                var NewInvoiceNumber = await _context.InvoicesModel.MaxAsync(x => x.Id);

                //add each invoice detail into the database
                foreach (var InvoiceDetail in InvoiceDetails)
                {
                    InvoiceDetailsModel newDetails = new InvoiceDetailsModel
                    {
                        ProductId = InvoiceDetail.Id,
                        InvoiceId = NewInvoiceNumber,
                        ProductName = ProductsData.Find(x => x.Id == InvoiceDetail.Id).Name,
                        ProductPrice = ProductsData.Find(x => x.Id == InvoiceDetail.Id).Price,
                        Quantity = InvoiceDetail.Quantity,
                        IsSaved = true
                    };
                    await _context.InvoiceDetailsModel.AddAsync(newDetails);
                    await _context.SaveChangesAsync();
                }
            }catch
            {
                ;
            }
        }

        public async Task RemoveInvoiceDetail(int invoiceId)
        {
            var invoiceDetails = await _context.InvoiceDetailsModel.Where(x => x.InvoiceId == invoiceId).ToListAsync();
            foreach (var invoiceDetail in invoiceDetails)
            {
                _context.InvoiceDetailsModel.Remove(invoiceDetail);
            }
            await _context.SaveChangesAsync();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoicesViewModel model)
        {
            //if model state is valid
            if (ModelState.IsValid)
            {
                //get the data of the customer chosen in the invoice
                var Customer = _context.CustomersModel.Where(x => x.Id == model.CustomersId).FirstOrDefault();

                //assign the information of invoice into a model
                InvoicesModel Invoice = new InvoicesModel
                {
                    Id = model.Id,
                    CustomersId = model.CustomersId,
                    CustomerAddress = Customer.Address,
                    CustomerEmail = Customer.Email,
                    CustomerName = Customer.Name,
                    CustomerHpNo = Customer.HpNo,
                    Date = model.Date,
                    TotalAmount = model.TotalAmount,
                    CancelStatus = false,
                    RefNo = model.RefNo,
                    PoNo = model.PoNo
                };
                //add all data of invoice into the database
                _context.Add(Invoice);
                await _context.SaveChangesAsync();

                //add all invoice detail into database
                await AddInvoiceDetail(JsonConvert.DeserializeObject<List<InvoiceDetailPost>>(model.InvoiceDetailsJson));
                //redirect back to invoice index page
                return RedirectToAction(nameof(Index));
            }
            //return submitted data to create invoice page
            return View(model);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //find specific invoice by using id
            var invoicesModel = await _context.InvoicesModel.FindAsync(id);
            //if invoice not found then return invoice not found page
            if (invoicesModel == null)
            {
                return NotFound();
            }

            //get all customer name (for dropdown in invoice)
            ViewBag.Customers = _context.CustomersModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            //get all products name (for dropdown in invoice)
            ViewBag.Products = _context.ProductsModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            //include all data of invoice in a model
            var invoicesViewModel = new InvoicesViewModel
            {
                Id = invoicesModel.Id,
                CustomersId = invoicesModel.CustomersId,
                CustomerAddress = invoicesModel.CustomerAddress,
                CustomerEmail = invoicesModel.CustomerEmail,
                CustomerName = invoicesModel.CustomerName,
                CustomerHpNo = invoicesModel.CustomerHpNo,
                Date = invoicesModel.Date,
                PoNo = invoicesModel.PoNo,
                RefNo = invoicesModel.RefNo,
                TotalAmount = invoicesModel.TotalAmount,
                Products = await _context.ProductsModel.ToListAsync(),
                InvoiceDetails = await _context.InvoiceDetailsModel.Where(x => x.InvoiceId == id).ToListAsync()
            };
            //set the id of the invoice
            ViewBag.InvoiceId = id?.ToString("0000000000") ?? "-";

            //return view with data of invoice
            return View(invoicesViewModel);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InvoicesViewModel model)
        {
            //if model state is valid
            if (ModelState.IsValid)
            {
                //get the customer of the invoice
                var Customer = _context.CustomersModel.Where(x => x.Id == model.CustomersId).FirstOrDefault();

                //put all information into a model
                InvoicesModel Invoice = new InvoicesModel
                {
                    Id = model.Id,
                    CustomersId = model.CustomersId,
                    CustomerAddress = Customer.Address,
                    CustomerEmail = Customer.Email,
                    CustomerName = Customer.Name,
                    CustomerHpNo = Customer.HpNo,
                    Date = model.Date,
                    TotalAmount = model.TotalAmount,
                    CancelStatus = false,
                    RefNo = model.RefNo,
                    PoNo = model.PoNo
                };

                //update the invoice
                _context.Update(Invoice);
                await _context.SaveChangesAsync();
                //remove all invoice detail
                await RemoveInvoiceDetail(id);
                try
                {
                    //add all invoice detail into database
                    await AddInvoiceDetail(JsonConvert.DeserializeObject<List<InvoiceDetailPost>>(model.InvoiceDetailsJson));
                }
                catch
                {

                }
                //redirect to invoice index page
                return RedirectToAction(nameof(Index));
            }
            //return all submitted data to edit invoice page
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int? id)
        {
            //find the specific invoice by using id
            var invoicesModel = await _context.InvoicesModel
                .FirstOrDefaultAsync(m => m.Id == id);
            //if invoice not found then return invoice not found page
            if (invoicesModel == null)
            {
                return NotFound();
            }

            //set invoice cancel status to true
            invoicesModel.CancelStatus = true;
            //update the specific invoice
            _context.InvoicesModel.Update(invoicesModel);
            await _context.SaveChangesAsync();

            //redirect to invoice index page
            return Redirect("/invoices/index"); ;
        }
    }
}
