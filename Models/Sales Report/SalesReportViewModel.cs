using System.ComponentModel.DataAnnotations;

namespace FSMS_asp.net.Models.Sales_Report
{
    public class SalesReportViewModel
    {
        [Display(Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        public ICollection<InvoicesModel>? Invoices { get; set; }
    }
}
