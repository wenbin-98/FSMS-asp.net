using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSMS_asp.net.Models.Quotation
{
    [Table("Quotation Details")]
    public class QuotationDetailsModel
    {
        [Key]
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ProductPrice { get; set; }
        public string ProductName { get; set; }
        public int? ProductId { get; set; }
        public int Quantity { get; set; }
        public int QuotationId { get; set; }
        [ForeignKey("QuotationId")]
        public virtual QuotationsModel Quotation { get; set; }
    }
}
