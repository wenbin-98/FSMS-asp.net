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
            var model = await _context.QuotationsModel.ToListAsync();

            return _context.QuotationsModel != null ?
                        View(model) :
                        Problem("Entity set 'ApplicationDbContext.InvoicesModel'  is null.");
        }

        // GET: Quotations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.QuotationsModel == null)
            {
                return NotFound();
            }

            var quotationsModel = await _context.QuotationsModel.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (quotationsModel == null)
            {
                return NotFound();
            }

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

            return View(quotationsViewModel);
        }

        // GET: Quotations/Create
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

            QuotationsViewModel quotationsViewModel = new QuotationsViewModel
            {
                Products = await _context.ProductsModel.ToListAsync()
            };

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
            if (ModelState.IsValid)
            {
                var Customer = _context.CustomersModel.Where(x => x.Id == model.CustomersId).FirstOrDefault();

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

                _context.Add(Quotation);
                await _context.SaveChangesAsync();

                await AddQuotationDetail(JsonConvert.DeserializeObject<List<QuotationDetailsPost>>(model.QuotationDetailsJson));
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Quotations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.QuotationsModel == null)
            {
                return NotFound();
            }

            var quotationsModel = await _context.QuotationsModel.FindAsync(id);
            if (quotationsModel == null)
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

            return View(quotationsViewModel);
        }

        // POST: Quotations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuotationsViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var Customer = _context.CustomersModel.Where(x => x.Id == model.CustomersId).FirstOrDefault();

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

                _context.Update(Quotation);
                await _context.SaveChangesAsync();

                await RemoveQuotationDetail(id);
                try
                {
                    await AddQuotationDetail(JsonConvert.DeserializeObject<List<QuotationDetailsPost>>(model.QuotationDetailsJson));
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
            if (id == null || _context.QuotationsModel == null)
            {
                return NotFound();
            }

            var quotationsModel = await _context.QuotationsModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quotationsModel == null)
            {
                return NotFound();
            }

            quotationsModel.CancelStatus = true;
            _context.QuotationsModel.Update(quotationsModel);
            await _context.SaveChangesAsync();

            return Redirect("/quotations/index"); ;
        }
    }
}
