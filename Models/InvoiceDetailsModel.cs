using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSMS_asp.net.Models
{
    [Table("Invoice Details")]
    public class InvoiceDetailsModel
    {
        [Key]
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ProductPrice { get; set; }
        public string ProductName { get; set; }
        public int? ProductId { get; set; }
        public int Quantity { get; set; }
        public bool IsSaved { get; set; } = false;

        public int InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual InvoicesModel Invoices { get; set; }
    }
}
