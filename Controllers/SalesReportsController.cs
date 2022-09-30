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

        [HttpPost]
        public async Task<JsonResult> Generate(DateTime startDate, DateTime endDate)
        {
            var invoices = await _context.InvoicesModel.Where(x => x.Date >= startDate && x.Date <= endDate).ToListAsync();
            return Json(invoices);
        }
    }
}
