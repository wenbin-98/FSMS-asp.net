using FSMS_asp.net.Data;
using FSMS_asp.net.Models.Sales_Report;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FSMS_asp.net.Controllers
{
    public class SalesReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            SalesReportViewModel model = new SalesReportViewModel()
            {
                StartDate = null,
                EndDate = null,
                Invoices = null
            };
            //return sales report page
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Generate(DateTime startDate, DateTime endDate)
        {
            //find all invoice between the start date and end date
            var invoices = await _context.InvoicesModel!.Where(x => x.Date >= startDate && x.Date <= endDate).ToListAsync();
            Decimal totalAmount = 0;

            //include all the invoice in the model
            SalesReportViewModel model = new SalesReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Invoices = invoices
            };

            //calculate the total amount of the sales report
            foreach (var item in invoices)
            {
                totalAmount += item.TotalAmount;
            }

            //set the total amount, start date and end date of the sales report
            ViewBag.totalAmount = totalAmount.ToString("0.00");
            ViewBag.startDate = startDate.ToString("dd/MM/yyyy");
            ViewBag.endDate = endDate.ToString("dd/MM/yyyy");

            //return the partial view 
            return PartialView("_salesReport", model);
        }
    }
}
