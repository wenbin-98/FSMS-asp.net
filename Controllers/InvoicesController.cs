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
            var model = await _context.InvoicesModel.ToListAsync();

            return _context.InvoicesModel != null ?
                        View(model) :
                        Problem("Entity set 'ApplicationDbContext.InvoicesModel'  is null.");
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.InvoicesModel == null)
            {
                return NotFound();
            }

            var invoicesModel = await _context.InvoicesModel.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (invoicesModel == null)
            {
                return NotFound();
            }

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

            return View(invoicesViewModel);
        }

        // GET: Invoices/Create
        public async Task<IActionResult> Create()
        {
            
            ViewBag.Customers = _context.CustomersModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            
            ViewBag.Products = _context.ProductsModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            int NewInvoiceId = 0;

            try
            {
                NewInvoiceId = await _context.InvoicesModel.MaxAsync(x => x.Id) + 1;
                NewInvoiceId.ToString("0000000000");
                ViewBag.NewInvoiceId = NewInvoiceId;
            }
            catch
            {
                ViewBag.NewInvoiceId = "0000000000";
            }

            InvoicesViewModel invoicesViewModel = new InvoicesViewModel
            {
                Products = await _context.ProductsModel.ToListAsync()
            };

            return View(invoicesViewModel);
        }

        [HttpPost]
        public async Task AddInvoiceDetail([FromBody] List<InvoiceDetailPost> InvoiceDetails)
        {
            try
            {
                var ProductsData = await _context.ProductsModel.ToListAsync();
                var NewInvoiceNumber = await _context.InvoicesModel.MaxAsync(x => x.Id);

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
            if (ModelState.IsValid)
            {
                var Customer = _context.CustomersModel.Where(x => x.Id == model.CustomersId).FirstOrDefault();

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

                _context.Add(Invoice);
                await _context.SaveChangesAsync();

                await AddInvoiceDetail(JsonConvert.DeserializeObject<List<InvoiceDetailPost>>(model.InvoiceDetailsJson));
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.InvoicesModel == null)
            {
                return NotFound();
            }

            var invoicesModel = await _context.InvoicesModel.FindAsync(id);
            if (invoicesModel == null)
            {
                return NotFound();
            }

            ViewBag.Customers = _context.CustomersModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            ViewBag.Products = _context.ProductsModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

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

            ViewBag.InvoiceId = id?.ToString("0000000000") ?? "-";

            return View(invoicesViewModel);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InvoicesViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var Customer = _context.CustomersModel.Where(x => x.Id == model.CustomersId).FirstOrDefault();

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

                _context.Update(Invoice);
                await _context.SaveChangesAsync();

                await RemoveInvoiceDetail(id);
                try
                {
                    await AddInvoiceDetail(JsonConvert.DeserializeObject<List<InvoiceDetailPost>>(model.InvoiceDetailsJson));
                }
                catch
                {

                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null || _context.InvoicesModel == null)
            {
                return NotFound();
            }

            var invoicesModel = await _context.InvoicesModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoicesModel == null)
            {
                return NotFound();
            }

            invoicesModel.CancelStatus = true;
            _context.InvoicesModel.Update(invoicesModel);
            await _context.SaveChangesAsync();

            return Redirect("/invoices/index"); ;
        }
    }
}
