using FSMS_asp.net.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FSMS_asp.net.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FSMS_asp.net.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; 
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> IndexAsync()
        {
            DateTime date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            List<Chart> data = new List<Chart>();

            for (var i = 5; i >= 0; i--)
            {
                var temp_first_day = firstDayOfMonth.AddMonths(-i);
                var temp_last_day = lastDayOfMonth.AddMonths(-i);
                //get all data of invoice
                var models = await _context.InvoicesModel.Where(x => x.Date < temp_last_day && x.Date > temp_first_day).ToListAsync();

                if (models.Any())
                {
                    decimal sum = 0;
                    foreach (var item in models)
                    {
                        sum += item.TotalAmount;
                    }

                    Chart temp = new Chart()
                    {
                        Year = temp_first_day.ToString("MMMM yyyy"),
                        Sales = sum
                    };
                    data.Add(temp);
                } else
                {
                    Chart temp = new Chart()
                    {
                        Year = temp_first_day.ToString("MMMM yyyy"),
                        Sales = 0
                    };
                    data.Add(temp);
                }
                
            }
            ViewBag.data = JsonSerializer.Serialize(data);
            //return view with data of invoice
            return _context.InvoicesModel != null ?
                        View(data) :
                        Problem("Entity set 'ApplicationDbContext.InvoicesModel'  is null.");
            //return Redirect("salesreports/index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}