using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text.Json.Serialization;

namespace FSMS_asp.net.Models.Quotation
{
    [Table("Quotations")]
    public class QuotationsModel
    {
        [Key]
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

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual List<QuotationDetailsModel>? QuotationDetails { get; set; }
    }
}
