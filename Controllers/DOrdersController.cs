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
            //get all data of DO
            var model = await _context.DOrdersModel.ToListAsync();

            //return view with data of DO
            return _context.DOrdersModel != null ?
                        View(model) :
                        Problem("Entity set 'ApplicationDbContext.DOrdersModel'  is null.");
        }

        // GET: DOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //find the specific DO by using id
            var dOrdersModel = await _context.DOrdersModel.Where(x => x.Id == id).FirstOrDefaultAsync();

            //if specific DO not found then return not found page
            if (dOrdersModel == null)
            {
                return NotFound();
            }

            //include all DO data in a model
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

            //return DO data with DO detail page
            return View(dOrdersViewModel);
        }

        // GET: DOrders/Create
        public async Task<IActionResult> Create()
        {
            //get all customer name (for dropdown in DO)
            ViewBag.Customers = _context.CustomersModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            //get all product name (for dropdown in DO)
            ViewBag.Products = _context.ProductsModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            int NewDOrderId = 0;
            //set the id of DO (use to show the current DO id in the page)
            try
            {
                NewDOrderId = await _context.DOrdersModel.MaxAsync(x => x.Id) + 1;
                ViewBag.NewDOrderId = NewDOrderId.ToString("0000000000");
            }
            catch
            {
                ViewBag.NewDOrderId = "0000000000";
            }

            //include all products data in the model
            DOrdersViewModel dOrdersViewModel = new DOrdersViewModel
            {
                Products = await _context.ProductsModel.ToListAsync()
            };

            //return all products and customer data to the create DO view 
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
            //if model state is valid
            if (ModelState.IsValid)
            {
                //get the data of the customer chosen in the DO
                var Customer = _context.CustomersModel.Where(x => x.Id == model.CustomersId).FirstOrDefault();

                //assign the information of DO into a model
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
                //add all data of DO into the database
                _context.Add(DOrders);
                await _context.SaveChangesAsync();
                //add all DO detail into database
                await AddDOrderDetail(JsonConvert.DeserializeObject<List<DOrderDetailsPost>>(model.DOrderDetailsJson));
                //redirect back to DO index page
                return RedirectToAction(nameof(Index));
            }
            //return submitted data to create DO page
            return View(model);
        }

        // GET: DOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //find specific DO by using id
            var dOrdersModel = await _context.DOrdersModel.FindAsync(id);
            //if DO not found then return DO not found page
            if (dOrdersModel == null)
            {
                return NotFound();
            }

            //get all customer name (for dropdown in DO)
            ViewBag.Customers = _context.CustomersModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            //get all products name (for dropdown in DO)
            ViewBag.Products = _context.ProductsModel.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            //include all data of DO in a model
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
            //set the id of the DO
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
            //return view with data of DO
            return View(dOrdersViewModel);
        }

        // POST: DOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DOrdersViewModel model)
        {
            //if model state is valid
            if (ModelState.IsValid)
            {
                //get the customer of the DO
                var Customer = _context.CustomersModel.Where(x => x.Id == model.CustomersId).FirstOrDefault();

                //put all information into a model
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

                //update the DO
                _context.Update(dOrders);
                await _context.SaveChangesAsync();
                //remove all DO detail
                await RemoveDOrderDetail(id);
                try
                {
                    //add all DO detail into database
                    await AddDOrderDetail(JsonConvert.DeserializeObject<List<DOrderDetailsPost>>(model.DOrderDetailsJson));
                }
                catch
                {

                }
                //redirect to DO index page
                return RedirectToAction(nameof(Index));
            }
            //return all submitted data to edit DO page
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int? id)
        {
            //find the specific DO by using id
            var dOrdersModel = await _context.DOrdersModel
                .FirstOrDefaultAsync(m => m.Id == id);
            //if DO not found then return DO not found page
            if (dOrdersModel == null)
            {
                return NotFound();
            }

            //set DO cancel status to true
            dOrdersModel.CancelStatus = true;
            //update the specific DO
            _context.DOrdersModel.Update(dOrdersModel);
            await _context.SaveChangesAsync();

            //redirect to DO index page
            return Redirect("/dOrders/index"); ;
        }
    }
}
