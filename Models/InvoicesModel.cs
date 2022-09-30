using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FSMS_asp.net.Models
{
    public class InvoicesModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Customer")]
        public int CustomersId { get; set; }

        [Required]
        [Display(Name = "Date")]
        [Column(TypeName = "Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Display(Name = "PO No.")]
        public string? PoNo { get; set; }

        [Display(Name = "Reference No.")]
        public string? RefNo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please add at least 1 Invoice Details.")]
        public decimal TotalAmount { get; set; }

        public bool CancelStatus { get; set; } = false;
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerHpNo { get; set; }
        public string? CustomerEmail { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual List<InvoiceDetailsModel>? InvoiceDetails { get; set; }
    }
}
