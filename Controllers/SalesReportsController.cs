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

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Generate(DateTime startDate, DateTime endDate)
        {
            var invoices = await _context.InvoicesModel!.Where(x => x.Date >= startDate && x.Date <= endDate).ToListAsync();
            Decimal totalAmount = 0;

            SalesReportViewModel model = new SalesReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Invoices = invoices
            };

            foreach (var item in invoices)
            {
                totalAmount += item.TotalAmount;
            }

            ViewBag.totalAmount = totalAmount.ToString("0.00");
            ViewBag.startDate = startDate.ToString("dd/MM/yyyy");
            ViewBag.endDate = endDate.ToString("dd/MM/yyyy");

            return PartialView("_salesReport", model);
        }
    }
}
