using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FSMS_asp.net.Models.Quotation
{
    public class QuotationsViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Date")]
        [Column(TypeName = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        public bool CancelStatus { get; set; } = false;
        [Display(Name = "Customer")]
        public int CustomersId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerHpNo { get; set; }
        public string? CustomerEmail { get; set; }

        public virtual List<QuotationDetailsModel>? QuotationDetails { get; set; }

        public ICollection<ProductsModel>? Products { get; set; }

        [NotMapped]
        public string QuotationDetailsJson { get; set; }
    }
}
