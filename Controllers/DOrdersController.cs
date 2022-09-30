using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FSMS_asp.net.Data;
using FSMS_asp.net.Models.Delivery_Order;
using FSMS_asp.net.Models;
using Newtonsoft.Json;
using FSMS_asp.net.Models.Quotation;

namespace FSMS_asp.net.Controllers
{
    public class DOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DOrders
        public async Task<IActionResult> Index()
        {
            var model = await _context.DOrdersModel.ToListAsync();

            return _context.DOrdersModel != null ?
                        View(model) :
                        Problem("Entity set 'ApplicationDbContext.DOrdersModel'  is null.");
        }

        // GET: DOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DOrdersModel == null)
            {
                return NotFound();
            }

            var dOrdersModel = await _context.DOrdersModel.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (dOrdersModel == null)
            {
                return NotFound();
            }

            var dOrdersViewModel = new DOrdersViewModel
            {
                Id = dOrdersModel.Id,
                CustomersId = dOrdersModel.CustomersId,
                CustomerAddress = dOrdersModel.CustomerAddress,
                CustomerEmail = dOrdersModel.CustomerEmail,
                CustomerName = dOrdersModel.CustomerName,
                CustomerHpNo = dOrdersModel.CustomerHpNo,
                Date = dOrdersModel.Date,
                PoNo = dOrdersModel.PoNo,
                RefNo = dOrdersModel.RefNo,
                TotalQuantity = dOrdersModel.TotalQuantity,
                TotalAmount = dOrdersModel.TotalAmount,
                Products = await _context.ProductsModel.ToListAsync(),
                DOrderDetails = await _context.DOrderDetailsModel.Where(x => x.DOrdersId == id).ToListAsync(),
                CancelStatus = dOrdersModel.CancelStatus,
            };

            return View(dOrdersViewModel);
        }

        // GET: DOrders/Create
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

            int NewDOrderId = 0;

            try
            {
                NewDOrderId = await _context.DOrdersModel.MaxAsync(x => x.Id) + 1;
                ViewBag.NewDOrderId = NewDOrderId;
            }
            catch
            {
                ViewBag.NewDOrderId = 0;
            }

            DOrdersViewModel dOrdersViewModel = new DOrdersViewModel
            {
                Products = await _context.ProductsModel.ToListAsync()
            };

            return View(dOrdersViewModel);
        }

        [HttpPost]
        public async Task AddDOrderDetail([FromBody] List<DOrderDetailsPost> DOrderDetails)
        {
            try
            {
                var ProductsData = await _context.ProductsModel.ToListAsync();
                var NewDOrderNumber = await _context.DOrdersModel.MaxAsync(x => x.Id);

                foreach (var DOrderDetail in DOrderDetails)
                {
                    DOrderDetailsModel newDetails = new DOrderDetailsModel
                    {
                        ProductId = DOrderDetail.Id,
                        DOrdersId = NewDOrderNumber,
                        ProductName = ProductsData.Find(x => x.Id == DOrderDetail.Id).Name,
                        ProductPrice = ProductsData.Find(x => x.Id == DOrderDetail.Id).Price,
                        Quantity = DOrderDetail.Quantity
                    };
                    await _context.DOrderDetailsModel.AddAsync(newDetails);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                ;
            }
        }

        public async Task RemoveDOrderDetail(int quotationId)
        {
            var DOrderDetails = await _context.DOrderDetailsModel.Where(x => x.DOrdersId == quotationId).ToListAsync();
            foreach (var DOrderDetail in DOrderDetails)
            {
                _context.DOrderDetailsModel.Remove(DOrderDetail);
            }
            await _context.SaveChangesAsync();
        }

        // POST: DOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DOrdersViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Customer = _context.CustomersModel.Where(x => x.Id == model.CustomersId).FirstOrDefault();

                DOrdersModel DOrders = new DOrdersModel
                {
                    Id = model.Id,
                    CustomersId = model.CustomersId,
                    CustomerAddress = Customer.Address,
                    CustomerEmail = Customer.Email,
                    CustomerName = Customer.Name,
                    CustomerHpNo = Customer.HpNo,
                    Date = model.Date,
                    TotalQuantity = model.TotalQuantity,
                    TotalAmount = model.TotalAmount,
                    CancelStatus = false,
                    RefNo = model.RefNo,
                    PoNo = model.PoNo
                };

                _context.Add(DOrders);
                await _context.SaveChangesAsync();

                await AddDOrderDetail(JsonConvert.DeserializeObject<List<DOrderDetailsPost>>(model.DOrderDetailsJson));
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: DOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DOrdersModel == null)
            {
                return NotFound();
            }

            var dOrdersModel = await _context.DOrdersModel.FindAsync(id);
            if (dOrdersModel == null)
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

            var dOrdersViewModel = new DOrdersViewModel
            {
                Id = dOrdersModel.Id,
                CustomersId = dOrdersModel.CustomersId,
                CustomerAddress = dOrdersModel.CustomerAddress,
                CustomerEmail = dOrdersModel.CustomerEmail,
                CustomerName = dOrdersModel.CustomerName,
                CustomerHpNo = dOrdersModel.CustomerHpNo,
                Date = dOrdersModel.Date,
                PoNo = dOrdersModel.PoNo,
                RefNo = dOrdersModel.RefNo,
                TotalQuantity = dOrdersModel.TotalQuantity,
                TotalAmount = dOrdersModel.TotalAmount,
                Products = await _context.ProductsModel.ToListAsync(),
                DOrderDetails = await _context.DOrderDetailsModel.Where(x => x.DOrdersId == id).ToListAsync()
            };

            int NewDOrdersId = 0;

            try
            {
                NewDOrdersId = await _context.QuotationsModel.MaxAsync(x => x.Id) + 1;
                ViewBag.NewDOrdersId = NewDOrdersId;
            }
            catch
            {
                ViewBag.NewDOrdersId = 0;
            }

            return View(dOrdersViewModel);
        }

        // POST: DOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DOrdersViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var Customer = _context.CustomersModel.Where(x => x.Id == model.CustomersId).FirstOrDefault();

                DOrdersModel dOrders = new DOrdersModel
                {
                    Id = model.Id,
                    CustomersId = model.CustomersId,
                    CustomerAddress = Customer.Address,
                    CustomerEmail = Customer.Email,
                    CustomerName = Customer.Name,
                    CustomerHpNo = Customer.HpNo,
                    Date = model.Date,
                    TotalQuantity = model.TotalQuantity,
                    TotalAmount = model.TotalAmount,
                    CancelStatus = false,
                    RefNo = model.RefNo,
                    PoNo = model.PoNo
                };

                _context.Update(dOrders);
                await _context.SaveChangesAsync();

                await RemoveDOrderDetail(id);
                try
                {
                    await AddDOrderDetail(JsonConvert.DeserializeObject<List<DOrderDetailsPost>>(model.DOrderDetailsJson));
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
            if (id == null || _context.DOrdersModel == null)
            {
                return NotFound();
            }

            var dOrdersModel = await _context.DOrdersModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dOrdersModel == null)
            {
                return NotFound();
            }

            dOrdersModel.CancelStatus = true;
            _context.DOrdersModel.Update(dOrdersModel);
            await _context.SaveChangesAsync();

            return Redirect("/dOrders/index"); ;
        }
    }
}
